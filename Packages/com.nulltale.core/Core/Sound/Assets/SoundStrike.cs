using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundStrike : SoundAsset, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clips.Random();

        [Range(0, 1)]
        public float        m_Volume = 1;
        public  float       m_StrikeTime = 1;
        public  AudioClip[] m_Clips;
        private float       m_LastInvoke;
        private int         m_Index;

        // =======================================================================
        public void Play(IAudioContext ctx)
        {
            Play(ctx, 1f);
        }

        public void Play(IAudioContext ctx, float vol)
        {
            var lastInvoke = m_LastInvoke;
            m_LastInvoke = Time.unscaledTime;
            
            m_Index ++;
            if (Time.unscaledTime - lastInvoke > m_StrikeTime)
                m_Index = 0;
            
            var clip = m_Clips.GetClamp(m_Index);
            ctx.AudioSource.PlayOneShot(clip, m_Volume);
        }
    }
}