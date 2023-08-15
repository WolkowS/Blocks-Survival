using System;
using System.Linq;
using UnityEngine;

namespace CoreLib.Tween
{
    public abstract class TweenOscillator : OscillatorBase
    {
        public  bool  m_RealTime;
        private float m_LastGet;

        protected float DeltaTime
        {
            get
            {
                var time = m_RealTime ? Time.unscaledTime : Time.time;

#if UNITY_EDITOR
                if (Application.isPlaying == false)
                    time = (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif

                var delta = time - m_LastGet;
                m_LastGet = time;

                return delta;
            }
        }

        private void Awake()
        {
            _startDeltaTime();
        }

        protected void _startDeltaTime()
        {
            m_LastGet = m_RealTime ? Time.unscaledTime : Time.time;
        }
    }
}
