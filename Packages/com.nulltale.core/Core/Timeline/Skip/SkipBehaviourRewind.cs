using CoreLib.Events;
using CoreLib.States;
using UnityEngine.Device;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class SkipBehaviourRewind : PlayableBehaviour
    {
        public SkipTrack        m_Track;
        public TimelineClip     m_Clip;
        public PlayableDirector m_Director;
        public float            m_RewindSpeed;

        private bool         m_IsPlaing;
        private bool         m_Invoked;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_IsPlaing)
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
            
            if (Application.isPlaying == false)
                return;

            if (m_Director.time + ControlBehaviour.s_TimeHorizon >= m_Clip.end)
                m_Director.time = m_Clip.end - ControlBehaviour.s_TimeHorizon;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_IsPlaing == false)
                return;
            m_IsPlaing = false;

            if (m_Invoked)
            {
                if (m_Director.playableGraph.IsValid())
                {
                    var rootPlayable = m_Director.playableGraph.GetRootPlayable(0);
                    if (rootPlayable.IsValid())
                        rootPlayable.SetSpeed(1f);
                }
            }
            m_Invoked = false;
            
            m_Track.UnSubscribe(_skip);
        }
        
        // =======================================================================
        private void _skip()
        {
            if (m_Invoked)
                return;

            m_Invoked = true;

            m_Director.playableGraph.GetRootPlayable(0).SetSpeed(m_RewindSpeed);
        }
    }
}