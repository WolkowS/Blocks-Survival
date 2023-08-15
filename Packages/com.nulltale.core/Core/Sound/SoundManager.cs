using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Malee;
using UnityEngine;
using UnityEngine.Audio;

namespace CoreLib.Sound
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SoundManager : MonoBehaviour, Core.IModule, IAudioContext
    {
        private static SoundManager s_Instance;
        public static SoundManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false || s_Instance == null)
                    s_Instance = FindObjectOfType<SoundManager>();
#endif

                return s_Instance;
            }
            private set => s_Instance = value;
        }

        [SerializeField]
        private AudioMixer m_Mixer;
        public AudioMixer Mixer => m_Mixer;

        [SerializeField]
        private  AudioMixerSnapshot m_StartSnapshot;
        [SerializeField]
        private  SnapshotBlending   m_SnapshotBlending = SnapshotBlending.Blend;

        [SerializeField]
        private AudioChannel m_Music;
        public AudioChannel Music => m_Music;
        [SerializeField]
        private AudioChannel m_Ambient;
        public AudioChannel Ambient => m_Ambient;
        [SerializeField]
        private SoundChannel m_Sound;
        public SoundChannel Sound => m_Sound;

        public AudioSource AudioSource => m_Sound.AudioSource;

        public MixerValue[] MixerValues => m_MixerValues;

        private MixerValue[]    m_MixerValues;

        private Dictionary<AudioMixerSnapshot, SnapshotData> m_Snapshots = new Dictionary<AudioMixerSnapshot, SnapshotData>();
        private bool                                         m_UpdateSnapshots;
        private Coroutine                                    m_UpdateSnapshotsCoroutine;

        // =======================================================================
        public class MixerData
        {
            public AudioMixerSnapshot       DefaultSnapshot;
            public AudioMixerGroup[]        AudioMixerGroups;
            public AudioMixerSnapshot[]     AudioSnapshots;
            public List<string>             ExposedParams;

            // =======================================================================
            public void SyncToMixer(AudioMixer mixer)
            {
                // fetch all audio groups under MASTER
                AudioMixerGroups = mixer.FindMatchingGroups("Master");

                // exposed Params
                var parameters = (Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);
                ExposedParams = new List<string>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var param     = parameters.GetValue(i);
                    var paramName = (string)param.GetType().GetField("name").GetValue(param);
                    ExposedParams.Add(paramName);
                }

                // snapshots
                AudioSnapshots = (AudioMixerSnapshot[])mixer.GetType().GetProperty("snapshots").GetValue(mixer, null);
                
                // default snapshot
                DefaultSnapshot = (AudioMixerSnapshot)mixer.GetType().GetProperty("startSnapshot").GetValue(mixer);
            }
        }

        public class SnapshotHandle : IDisposable
        {
            internal SnapshotData Owner;

            private float m_Weight;
            public float Weight
            {
                get => m_Weight;
                set
                {
                    if (m_Weight == value)
                        return;

                    m_Weight = value;
                    Owner.UpdateWeight();
                }
            }

            public void Dispose()
            {
                Owner.m_Handles.Remove(this);
                Owner.UpdateWeight();
            }
        }

        public class SnapshotData
        {
            public  AudioMixerSnapshot Snapshot;

            internal List<SnapshotHandle> m_Handles = new List<SnapshotHandle>();

            internal float Weight;

            // =======================================================================
            internal SnapshotHandle OpenHandle()
            {
                var handle = new SnapshotHandle() { Owner = this };
                m_Handles.Add(handle);
                return handle;
            }

            internal void UpdateWeight()
            {
                var value = m_Handles.IsEmpty() ? 0f : m_Handles.Max(n => n.Weight);

                if (Weight == value)
                    return;

                Weight = value;
                Instance._supportSnapshotsUpdate();
            }

            public SnapshotData(AudioMixerSnapshot snapshot)
            {
                Snapshot = snapshot;
            }
        }

        [Serializable]
        public enum SnapshotBlending
        {
            Blend,
            Override
        }

        // =======================================================================
        public void Init()
        {
            // set as main instance
            Instance = this;

            // move audio sources to the main camera
            m_Music.transform.SetParent(Core.Camera.transform, false);
            m_Ambient.transform.SetParent(Core.Camera.transform, false);
            m_Sound.transform.SetParent(Core.Camera.transform, false);

            // setup mixer value 
            m_MixerValues = GetComponentsInChildren<MixerValue>();
            this.Delayed(() =>
            {
                foreach (var mixerValue in m_MixerValues)
                    mixerValue.Init();

                m_StartSnapshot.TransitionTo(0.1f);
            });

            m_Snapshots = new Dictionary<AudioMixerSnapshot, SnapshotData>();
        }

        internal SnapshotHandle OpenSnapshotHandle(AudioMixerSnapshot snapshot)
        {
            // update weight
            if (m_Snapshots.TryGetValue(snapshot, out var snapshotData) == false)
            {
                snapshotData = new SnapshotData(snapshot);
                m_Snapshots.Add(snapshot, snapshotData);
            }

            return snapshotData.OpenHandle();
        }

        private void _supportSnapshotsUpdate()
        {
            m_UpdateSnapshots   = true;

            if (m_UpdateSnapshotsCoroutine == null)
                m_UpdateSnapshotsCoroutine = StartCoroutine(_snapshotsUpdate());
        }
        
        private IEnumerator _snapshotsUpdate()
        {
            yield return null;

            // delayed update
            while (m_UpdateSnapshots)
            {
                var transitionTime = Time.smoothDeltaTime;
                var snapshots      = m_Snapshots.Values.Where(n => n.Weight > 0).ToArray();

                if (snapshots.IsEmpty())
                {
                    m_StartSnapshot.TransitionTo(transitionTime);
                    m_UpdateSnapshots = false;
                    yield return null;
                    continue;
                }

                // apply snapshot transition
                switch (m_SnapshotBlending)
                {
                    case SnapshotBlending.Blend:
                    {
                        var deviationWeight = snapshots.Max(n => n.Weight);
                        var fullWeight      = snapshots.Sum(n => n.Weight);

                        var eSnapshots = snapshots.Select(n => n.Snapshot);
                        var eWeights   = snapshots.Select(n => (n.Weight / fullWeight) * deviationWeight);

                        if (deviationWeight < 1)
                        {
                            eSnapshots = eSnapshots.Prepend(m_StartSnapshot);
                            eWeights   = eWeights.Prepend(1f - deviationWeight);
                        }

                        Mixer.TransitionToSnapshots(eSnapshots.ToArray(), eWeights.ToArray(), transitionTime);
                    } break;

                    case SnapshotBlending.Override:
                    {
                        var majorSnapshot = snapshots.MaxByOrDefault(n => n.Weight);
                        if (majorSnapshot.Weight >= 1f)
                        {
                            majorSnapshot.Snapshot.TransitionTo(transitionTime);
                            break;
                        }

                        Mixer.TransitionToSnapshots(new AudioMixerSnapshot[] { m_StartSnapshot, majorSnapshot.Snapshot }, new float[] { majorSnapshot.Weight.OneMinus(), majorSnapshot.Weight }, transitionTime);
                    } break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }


                m_UpdateSnapshots = false;
                yield return null;
            }

            m_UpdateSnapshotsCoroutine = null;
        }

        // -----------------------------------------------------------------------
        internal static IEnumerator TransitionCoroutine(AudioSource audioSource, AudioClip clip, float volume, AnimationCurve leave, AnimationCurve enter, float time = 0f)
        {
            // run leave transition, if playing
            if (audioSource.isPlaying)
                yield return volumeCurve(leave, audioSource.volume);

            // run enter transition, if clip not null
            audioSource.clip = clip;
            if (clip != null)
            {
                audioSource.time = time;
                audioSource.Play();
                yield return volumeCurve(enter, volume);
            }
            else
                audioSource.Stop();


            // -----------------------------------------------------------------------
            IEnumerator volumeCurve(AnimationCurve curve, float desiredVolume)
            {
                var lastKey = curve.keys.Last();
                var duration = lastKey.time;
                for (var currentTime = 0f; currentTime < duration; currentTime += Time.deltaTime)
                {
                    audioSource.volume = desiredVolume * curve.Evaluate(currentTime);
                    yield return null;
                }

                // apply last key
                audioSource.volume = desiredVolume * lastKey.value;
            }
        }
    }
}