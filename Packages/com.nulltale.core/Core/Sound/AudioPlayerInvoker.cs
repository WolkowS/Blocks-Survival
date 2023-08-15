using UnityEngine;

namespace CoreLib.Sound
{
    [RequireComponent(typeof(SoundPlayer))]
    [DefaultExecutionOrder(-1)]
    public sealed class AudioPlayerInvoker : MonoBehaviour
    {
        private SoundPlayer m_Player;
        [SerializeField]
        private SoundAsset m_Audio;

        public Optional<float>  m_Volume;

        // =======================================================================
        private void Awake()
        {
            m_Player = GetComponent<SoundPlayer>();
        }

        public void Invoke()
        {
            if (m_Volume)
                m_Player.Play(m_Audio, m_Volume);
            else
                m_Player.Play(m_Audio);
        }

    }
}