using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundPlayer : MonoBehaviour, IAudioContext
    {
        [SerializeField]
        private Context                 m_Context;
        [SerializeField]
        [ShowIf(nameof(m_Context), Context.Source)]
        private AudioSource             m_AudioSource;
        public AudioSource AudioSource => m_AudioSource;

        // =======================================================================
        [Serializable]
        public enum Context
        {
            Global,
            Source
        }

        // =======================================================================
        public void AudioEvent(SoundAsset audioProvider) => Play(audioProvider);

        public void Play(SoundAsset asset)
        {
            if (asset.IsNull())
                return;

            PlayAudio(asset.Audio);
        }

        public virtual void PlayAudio(IAudio audio)
        {
            if (audio == null)
                return;

            switch (m_Context)
            {
                case Context.Global:
                    audio.Play(SoundManager.Instance);
                    break;
                case Context.Source:
                    audio.Play(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}