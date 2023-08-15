using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundSequence : SoundAsset, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clips.Random();

        [Range(0, 1)]
        public float m_Volume = 1;
        public AudioClip[] m_Clips;

        private int _index = 0;
        
        // =======================================================================
        public void Play(IAudioContext ctx)
        {
            if (_index >= m_Clips.Length)
                _index = 0;
            
            ctx.AudioSource.PlayOneShot(m_Clips[_index], m_Volume);
            _index ++;
        }

        public void Play(IAudioContext ctx, float vol)
        {
            if (_index >= m_Clips.Length)
                _index = 0;
            
            ctx.AudioSource.PlayOneShot(m_Clips[_index], m_Volume * vol);
            _index ++;
        }
    }
}