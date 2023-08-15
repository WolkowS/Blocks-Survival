using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundRandom : SoundAsset, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clips.Random();

        [Range(0, 1)]
        public float m_Volume = 1;
        public AudioClip[] m_Clips;

        // =======================================================================
        public void Play(IAudioContext ctx)
        {
            var clip = m_Clips.Random();
            if (clip == null)
                return;
            
            ctx.AudioSource.PlayOneShot(clip, m_Volume);
        }

        public void Play(IAudioContext ctx, float vol)
        {
            var clip = m_Clips.Random();
            if (clip == null)
                return;
            
            ctx.AudioSource.PlayOneShot(clip, m_Volume * vol);
        }
    }
}