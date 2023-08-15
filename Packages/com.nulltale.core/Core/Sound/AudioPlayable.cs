using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace CoreLib.Sound
{
    public class AudioPlayable : MonoBehaviour
    {
        private PlayableGraph       m_Graph;
        private AudioPlayableOutput m_AudioOutput;
        private AudioMixerPlayable  m_AudioMixer;
        private List<PlayHandle>    m_Handles = new List<PlayHandle>();

        // =======================================================================
        public class PlayHandle : IDisposable
        {
            internal bool              Disposed { get; set; }
            private  AudioPlayable     m_Owner;
            private  int               m_Input;
            private  AudioClipPlayable m_Clip;

            public AudioClip Clip
            {
                get => m_Clip.GetClip();
                set => m_Clip.SetClip(value);
            }

            public float Volume
            {
                get => m_Owner.m_AudioMixer.GetInputWeight(m_Input);
                set => m_Owner.m_AudioMixer.SetInputWeight(m_Input, value);
            }

            public float Pitch
            {
                get => (float)m_Clip.GetSpeed();
                set => m_Clip.SetSpeed(value);
            }

            public bool IsLooped
            {
                get => m_Clip.GetLooped();
                set => m_Clip.SetLooped(value);
            }

            public bool Play
            {
                get => m_Clip.IsChannelPlaying();
                set
                {
                    if (value)
                        m_Clip.Play();
                    else
                        m_Clip.Pause();
                }
            }
            
            // =======================================================================
            internal void Open(AudioClip clip, float volume = 1f, float pitch = 1f)
            {
                m_Clip.SetClip(clip);
                m_Clip.SetSpeed(pitch);
                m_Owner.m_AudioMixer.ConnectInput(m_Input, m_Clip, 0, volume);
                Disposed = false;
            }

            public void Dispose()
            {
                if (Disposed)
                    return;

                m_Owner.m_AudioMixer.DisconnectInput(m_Input);
                m_Clip.Pause();
                Disposed = true;
            }

            public PlayHandle(AudioPlayable owner, int input)
            {
                m_Owner = owner;
                m_Input = input;
                m_Clip  = AudioClipPlayable.Create(m_Owner.m_Graph, null, true);
                m_Clip.Pause();
            }
        }

        // =======================================================================
        private void Awake()
        {
            m_Graph = PlayableGraph.Create();

            m_AudioOutput = AudioPlayableOutput.Create(m_Graph, "Audio", GetComponent<AudioSource>());
            m_AudioMixer  = AudioMixerPlayable.Create(m_Graph);
            m_AudioOutput.SetSourcePlayable(m_AudioMixer);
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.DSPClock);
        }

        private void OnEnable()
        {
            m_Graph.Play();
        }

        private void OnDisable()
        {
            m_Graph.Stop();
        }

        private void OnDestroy()
        {
            if (m_Graph.IsValid() == false)
                return;

            foreach (var handle in m_Handles)
                handle.Disposed = false;

            m_Graph.Destroy();
        }

        public PlayHandle CreatePlayHandle(AudioClip clip = null, float volume = 1f, float pitch = 1f)
        {
            var handle = m_Handles.FirstOrDefault(n => n.Disposed);
            if (handle == null)
            {
                handle = new PlayHandle(this, m_Handles.Count);
                m_Handles.Add(handle);
                m_AudioMixer.SetInputCount(m_Handles.Count);
            }

            handle.Open(clip, volume, pitch);
            return handle;
        }
    }
}