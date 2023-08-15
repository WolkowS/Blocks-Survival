using System;
using CoreLib.Commands;
using CoreLib.States;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace CoreLib.Timeline
{
    public class CommandBehaviour : PlayableBehaviour
    {
        public static double s_TimeHorizon => Time.maximumDeltaTime + 0.01f;
        
        // =======================================================================
        public  Cmd                         m_Cmd;
        public  PlayableDirector            m_Director;
        public  TimelineClip                m_Clip;
        private bool                        m_IsPlaing;
        public  Optional<Object>            m_Args;
        public  bool                        m_Wait;
        private bool                        m_DisallowLeave;
        private Cmd.Handle                  m_Handle;
        private double                      m_InitialSpeed;
        private bool                        m_OnPause;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_IsPlaing)
                return;

            m_Handle = null;
            m_IsPlaing = true;
            
            PrepareFrame(playable, info);

            if (m_Cmd.RuntimeOnly && Application.isPlaying == false)
                return;
            
            if (m_Args.Enabled && m_Cmd is Cmd.IArgsCall argsCall)
                argsCall.Invoke(m_Args.Value); 
            else
            if (m_Cmd is Cmd.IVoidCall voidCall) 
                voidCall.Invoke();
            
            m_Handle = m_Cmd._handle;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (m_Wait == false)
                return;
                
            if (m_Handle == null)
                return;
                
            m_DisallowLeave = m_Handle.IsOpen;

            var timeEdge = m_Director.time + s_TimeHorizon >= m_Clip.end;
            if (m_DisallowLeave && timeEdge && m_OnPause == false)
            {
                m_Director.time = m_Clip.end - s_TimeHorizon;

                var rootPlayable = playable.GetGraph().GetRootPlayable(0);
                m_InitialSpeed = rootPlayable.GetSpeed();
                rootPlayable.SetSpeed(0d);
                
                m_OnPause = true;
            }

            _unPause(playable);
        }

        private void _unPause(Playable playable)
        {
            if (m_DisallowLeave == false && m_OnPause)
            {
                var rootPlayable = playable.GetGraph().GetRootPlayable(0);
                rootPlayable.SetSpeed(m_InitialSpeed);
                m_OnPause = false;
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_Director == null)
                return;
            
            if (m_DisallowLeave && m_OnPause == false)
            {
                m_Director.time = m_Clip.end - (1d / 60d);
                
                var rootPlayable = playable.GetGraph().GetRootPlayable(0);
                m_InitialSpeed = rootPlayable.GetSpeed();
                rootPlayable.SetSpeed(0d);
                
                m_OnPause = true;
                return;
            }
            
            _unPause(playable);

            if (m_IsPlaing)
                m_IsPlaing = false;
        }
    }
}
