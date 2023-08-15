using System;
using CoreLib.Events;
using CoreLib.States;
using UnityEngine.Device;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class SkipBehaviourSkip : PlayableBehaviour
    {
        public SkipTrack m_Track;
        public PlayableDirector   m_Director;
        public TimelineClip       m_Clip;

        private bool m_IsPlaing;
        private bool m_Invoked;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_IsPlaing)
                return;

            if (m_Invoked && Math.Abs(m_Director.time - m_Clip.end) < 0.0001d)
                return;
            
            m_IsPlaing = true;
            m_Invoked  = false;

            m_Track.Subscribe(_skip);
        }
        
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (m_Track._wait == false)
                return;
            
            if (m_Invoked)
                return;
            
            if (m_IsPlaing == false)
                return;
            
            if (Application.isPlaying == false)
                return;

            if (m_Director.time + ControlBehaviour.s_TimeHorizon >= m_Clip.end)
                m_Director.time = m_Clip.end - ControlBehaviour.s_TimeHorizon;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_IsPlaing == false)
                return;

            m_Track.UnSubscribe(_skip);
            m_IsPlaing         =  false;
        }
        
        // =======================================================================
        private void _skip()
        {
            if (m_Invoked)
                return;

            m_Invoked = true;

            m_Director.time = m_Clip.end;
        }
    }
}