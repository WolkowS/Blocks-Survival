using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundMusic : SoundAsset, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clip;

        [Range(0, 1)]
        public float     m_Volume = 1;
        public AudioClip m_Clip;

        public Optional<ParticleSystem.MinMaxCurve> m_Time;
        public Optional<AnimationCurve>             m_Enter = new Optional<AnimationCurve>(AnimationCurve.Linear(0, 0, 1, 1), false);
        public Optional<AnimationCurve>             m_Leave = new Optional<AnimationCurve>(AnimationCurve.Linear(1, 1, 0, 0), false);

        // =======================================================================
        public void Play(IAudioContext context)
        {
            var time = m_Time.Enabled ? m_Time.Value.Evaluate() : 0f;

            switch (context)
            {
                case SoundManager sm:
                {
                    var source = sm.Music.AudioSource;
                    if (source.clip == m_Clip)
                        break;
                    
                    sm.StartCoroutine(SoundManager.TransitionCoroutine(source, m_Clip, m_Volume, m_Leave ? m_Leave.Value : sm.Music.Leave, m_Enter ? m_Enter.Value : sm.Music.Enter, time));
                } break;
                case IAudioChannelContext channel:
                {
                    var source = channel.AudioSource;
                    if (source.clip == m_Clip)
                        break;
                    
                    ((MonoBehaviour)channel).StartCoroutine(SoundManager.TransitionCoroutine(source, m_Clip, m_Volume, m_Leave ? m_Leave.Value : channel.Leave, m_Enter ? m_Enter.Value : channel.Enter, time));
                } break;
                default:
                {
                    var source = context.AudioSource;
                    if (source.clip == m_Clip)
                        break;

                    source.clip   = m_Clip;
                    source.volume = m_Volume;
                    source.time   = time;
                    source.Play();
                } break;
            }
        }

        public void Play(IAudioContext ctx, float vol)
        {
            throw new System.NotImplementedException();
        }
    }
}