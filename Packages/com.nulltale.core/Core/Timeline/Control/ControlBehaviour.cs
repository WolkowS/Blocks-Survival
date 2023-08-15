using System;
using CoreLib.States;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class ControlBehaviour : PlayableBehaviour
    {
        public static double s_TimeHorizon => Time.maximumDeltaTime + 0.001f;
        
        // =======================================================================
        public GlobalStateBase m_State;
        public Condition   m_While;
        public Mode        m_Mode;

        public PlayableDirector m_Director;

        private bool     m_DisallowLeave;
        private bool     m_IsPlaing;

        public  TimelineClip m_Clip;
        private bool         m_OnHold;
        public  float        m_ReleaseTime;
        private float        m_ReleaseTimeCurrent; 
        private double       m_InitialSpeed;
        public GameObject    m_Activate;


        // =======================================================================
        [Serializable]
        public enum Condition
        {
            Never,

            Open,
            Close,
            Always
        }

        [Serializable]
        public enum Mode
        {
            Repeat,
            Hold,
            Pause
        }

        [Serializable]
        public enum Notification
        {
            Enter,
            Leave
        }

        public class ControlNotification : INotification
        {
            public PropertyName     id { get; } = new PropertyName(nameof(ControlBehaviour));
            public ControlBehaviour Behaviour;
            public Notification     Notification;

            public override string ToString()
            {
                return $"{Behaviour.m_Clip.displayName} {Notification}";
            }
        }

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_IsPlaing)
                return;

            m_IsPlaing = true;
            PrepareFrame(playable, info);

            if (m_Activate != null)
                m_Activate.gameObject.SetActive(true);
            //info.output.PushNotification(playable, new ControlNotification() { Behaviour = this, Notification = Notification.Enter });
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (m_Director == null)
                return;

            if (m_State == null)
            {
                Debug.LogWarning($"Loop State must be set from the director: ({m_Director.name})");
                return;
            }
            var end = m_Director.time + s_TimeHorizon >= m_Clip.end;

            var wait = m_While switch
            {
                Condition.Never  => false,
                Condition.Open   => m_State.IsOpen,
                Condition.Close  => m_State.IsOpen == false,
                Condition.Always => true,
                _                    => throw new ArgumentOutOfRangeException()
            };

#if UNITY_EDITOR
            // editor behaviour
            if (Application.isPlaying == false)
            {
                switch (m_Mode)
                {
                    case Mode.Repeat:
                        wait = true;
                        break;
                    case Mode.Hold:
                        wait = false;
                        break;
                    case Mode.Pause:
                        break;
                }
            }
#endif

            switch (m_Mode)
            {
                case Mode.Repeat:
                {
                    m_DisallowLeave = wait;

                    // trigger at the end
                    if (end == false)
                        return;

                    if (wait == false)
                        return;

                    m_Director.time = m_Clip.start;
                } break;

                case Mode.Hold:
                {
                    if (m_OnHold == false)
                    {
                        if (end == false)
                            return;

                        if (wait == false)
                            return;

                        // set hold and pause
                        m_OnHold = true;
                        m_DisallowLeave = true;
                        m_ReleaseTimeCurrent = m_ReleaseTime;
                        _pause();
                    }
                    else
                    {
                        if (wait)
                        {
                            m_ReleaseTimeCurrent = m_ReleaseTime;
                            return;
                        }

                        if (m_ReleaseTimeCurrent > 0)
                        {
                            m_ReleaseTimeCurrent -= info.deltaTime;
                            return;
                        }
                        
                        // restore speed, allow to leave the behaviour
                        m_DisallowLeave = false;
                        playable.GetGraph().GetRootPlayable(0).SetSpeed(m_InitialSpeed);
                    }
                } break;

                case Mode.Pause:
                {
                    if (m_OnHold == false)
                    {
                        if (wait == false)
                            return;

                        // set hold and pause
                        m_OnHold = true;
                        m_DisallowLeave = true;
                        m_ReleaseTimeCurrent = m_ReleaseTime;
                        _pause();
                    }
                    else
                    {
                        if (wait)
                        {
                            m_ReleaseTimeCurrent = m_ReleaseTime;
                            return;
                        }

                        if (m_ReleaseTimeCurrent > 0)
                        {
                            m_ReleaseTimeCurrent -= info.deltaTime;
                            return;
                        }
                        
                        // restore speed, allow to leave the behaviour
                        m_DisallowLeave = false;
                        playable.GetGraph().GetRootPlayable(0).SetSpeed(m_InitialSpeed);
                    }
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // -----------------------------------------------------------------------
            void _pause()
            {
                var rootPlayable = playable.GetGraph().GetRootPlayable(0);
                m_InitialSpeed = rootPlayable.GetSpeed();
                rootPlayable.SetSpeed(0d);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_Director == null)
                return;

            if (m_DisallowLeave)
            {
                m_Director.time = m_Clip.end - (1d / 60d);
                return;
            }

            if (m_OnHold)
            {
                m_OnHold = false;
                var root = playable.GetGraph().GetRootPlayable(0);
                root.SetSpeed(m_InitialSpeed);
            }

            if (m_IsPlaing)
            {
                m_IsPlaing = false;
                if (m_Activate != null)
                    m_Activate.gameObject.SetActive(false);
                //info.output.PushNotification(playable, new ControlNotification() { Behaviour = this, Notification = Notification.Leave });
            }
        }
    }
}
