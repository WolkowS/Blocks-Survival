using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ProgressBehaviour : PlayableBehaviour
    {
        public Optional<Vector2>          m_Range = new Optional<Vector2>(new Vector2(0, 1), false);
        public Optional<AnimationCurve01> m_Lerp;
        [NonSerialized]
        public TimelineClip               m_Clip;
        [NonSerialized]
        public PlayableDirector           m_Director;
        
        public  float Value => m_Value;
        private float m_Value;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_Value = 0f;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            var scale = (float)((m_Director.time - m_Clip.start) / m_Clip.duration).Clamp01();
            if (m_Lerp.Enabled)
                scale = m_Lerp.Value.Evaluate(scale);
            
            var val   = m_Range.Enabled ? Mathf.LerpUnclamped(m_Range.Value.x, m_Range.Value.y, scale) : scale;
            
            m_Value = val;
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_Value = 1f;
        }

    }
}