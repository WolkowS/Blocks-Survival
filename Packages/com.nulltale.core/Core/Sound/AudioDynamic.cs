using System;
using UnityEngine;

namespace CoreLib.Sound
{
    [Serializable]
    public class AudioDynamic :  MonoBehaviour
    {
        [SerializeField]
        private SoundAsset            m_AudioProvider;
        private IAudio                   m_Audio;
        private AudioPlayable.PlayHandle m_PlayHandle;

        public  AudioClip Clip => m_Audio.Clip;

        [SerializeField]
        private SoundPlayer m_Context;

        private AudioPlayable   m_AudioPlayable;
        private bool m_IsPlaying;

        [SerializeField] [Range(0, 1)]
        private float m_Volume;
        [SerializeField] [Range(-3, 3)]
        private float m_Pitch = 1;

        public float Volume
        {
            get => m_PlayHandle.Volume;
            set
            {
                if (m_Volume == value)
                    return;

                m_Volume = value;

                _validateVolume();
            }
        }
        public float Pitch
        {
            get => m_Pitch;
            set
            {
                if (m_Pitch == value)
                    return;

                m_PlayHandle.Pitch = m_Pitch;
            }
        }

        // =======================================================================
        public void Awake()
        {
            // get or create audio playable
            if (m_Context.AudioSource.TryGetComponent<AudioPlayable>(out m_AudioPlayable) == false)
                m_AudioPlayable = m_Context.AudioSource.gameObject.AddComponent<AudioPlayable>();
        }

        private void OnEnable()
        {
            m_PlayHandle = m_AudioPlayable.CreatePlayHandle();
            _validateVolume();
        }

        private void OnDisable()
        {
            m_PlayHandle.Dispose();
            m_PlayHandle = null;
            m_IsPlaying = false;
        }

        public void OnDestroy()
        {
            m_PlayHandle?.Dispose();
            m_PlayHandle = null;
        }

        private void OnValidate()
        {
            if (Application.isPlaying == false)
                return;

            if (m_PlayHandle == null)
                return;

            _validateVolume();
            
            m_PlayHandle.Pitch = m_Pitch;
        }

        // -----------------------------------------------------------------------
        private void _validateVolume()
        {
            if (m_PlayHandle == null)
                return;

            switch (m_IsPlaying)
            {
                // turn on or off playing
                case false when m_Volume > 0f:
                    m_IsPlaying         = true;
                    m_Audio           = m_AudioProvider.Audio;
                    m_PlayHandle.Clip = m_Audio.Clip;
                    m_PlayHandle.Play = true;
                    break;
                case true when m_Volume <= 0f:
                    m_IsPlaying         = false;
                    m_PlayHandle.Play = false;
                    break;
            }

            m_PlayHandle.Volume = m_Volume;
        }
    }
}