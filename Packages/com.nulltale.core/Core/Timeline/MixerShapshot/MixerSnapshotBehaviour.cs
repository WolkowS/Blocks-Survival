using System;
using CoreLib.Sound;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    [Serializable]
    public class MixerSnapshotBehaviour : PlayableBehaviour
    {
        public AudioMixerSnapshot                   Snapshot;
        [Range(0.0f, 1.0f)]
        public float                                Weight = 1.0f;

        [NonSerialized]
        public SoundManager.SnapshotHandle m_SnapshotHandle;

        // =======================================================================
        public override void OnGraphStart(Playable playable)
        {
            if (m_SnapshotHandle == null)
                m_SnapshotHandle = SoundManager.Instance.OpenSnapshotHandle(Snapshot);
        }

        public override void OnGraphStop(Playable playable)
        {
            if (m_SnapshotHandle != null)
            {
                m_SnapshotHandle.Dispose();
                m_SnapshotHandle = null;
            }
        }
    }
}