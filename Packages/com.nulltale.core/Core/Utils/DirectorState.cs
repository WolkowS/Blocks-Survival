using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    [RequireComponent(typeof(PlayableDirector))]
    public class DirectorState : MonoBehaviour, IPlayableValue, IPlayableValueHandle, IToggle
    {
        public bool   m_Immediate;
        [HideIf(nameof(m_Immediate))]
        public double m_Speed     = 1d;
        [HideIf(nameof(m_Immediate))]
        public double m_UpSpeed   = 1d;
        [HideIf(nameof(m_Immediate))]
        public double m_DownSpeed = 1d;
        [HideIf(nameof(m_Immediate))]
        public float  m_UpLerp;
        [HideIf(nameof(m_Immediate))]
        public float  m_DownLerp;

        [HideIf(nameof(m_Immediate))]
        public Optional<AnimationCurve> m_Interpolation;

        private PlayableDirector m_Director;

        private double m_Time;
        private double m_Duration;

        [HideIf(nameof(m_Immediate))]
        public bool m_UnScaledTime = true;

        [SerializeField] [Range(0, 1)]
        private double m_DesiredTime;

        public double DesiredTime
        {
            get => m_DesiredTime;
            set
            {
                var val = value.Clamp01();
                if (val == m_DesiredTime)
                    return;

                m_DesiredTime = val;
                enabled = true;
            }
        }
        
        public bool IsOn
        {
            get => m_DesiredTime == 1d;
            set => SetDesiredTime(value);
        }
        
        public bool IsComplete => enabled == false;

        public PlayableDirector Director => m_Director;

        // =======================================================================
        public void SetTime(float normalizedTime)
        {
            DesiredTime = normalizedTime;
            
            m_Time = m_DesiredTime; 
            m_Director.time = m_Interpolation.Enabled ? m_Interpolation.Value.Evaluate((float)(m_Time / m_Duration)) * m_Duration : m_Time;
            m_Director.Evaluate();
        }
        
        public void SetDesiredTime(float normalizedTime)
        {
            DesiredTime = normalizedTime;
        }

        public void SetDesiredTime(bool state)
        {
            DesiredTime = state ? 1d : 0d;
            
            if (m_Immediate)
            {
                m_Director.time = DesiredTime;
                m_Director.Evaluate();
            }
        }
        
        public void Fire()
        {
            Fire(1);
        }
        
        public void Fire(float time)
        {
            SetTime(time);
            
            DesiredTime = 0d;
        }

        private void Awake()
        {
            m_Director                   = GetComponent<PlayableDirector>();
            m_Director.Stop();

            m_Duration                   = m_Director.duration;
            m_Time                       = m_Duration * DesiredTime;
            m_Director.timeUpdateMode    = DirectorUpdateMode.Manual;
            m_Director.extrapolationMode = DirectorWrapMode.Hold;
        }

        private void OnEnable()
        {
            m_Director.time = m_Time;
            m_Director.Evaluate();
        }

        private void OnValidate()
        {
            if (m_Director == null)
                return;

            if (m_Director.duration * DesiredTime != m_Director.time)
                enabled = true;
        }

        private void Update()
        {
            var desiredTime = m_Director.duration * DesiredTime;

            if (m_Time == desiredTime)
            {
                enabled = false;
                return;
            }
            
            if (m_Immediate)
            {
                m_Director.time = desiredTime;
                m_Director.Evaluate();
                return;
            }

            var deltaTime  = m_UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (desiredTime > m_Director.time)
            {
                m_Time += m_UpSpeed * m_Speed * deltaTime;
                m_Time = Mathf.Lerp((float)m_Time, (float)desiredTime, m_UpLerp * deltaTime);
                
                if (m_Time > desiredTime)
                    m_Time = desiredTime;
            }
            else
            {
                m_Time -= m_DownSpeed * m_Speed * deltaTime;
                m_Time = Mathf.Lerp((float)m_Time, (float)desiredTime, m_DownLerp * deltaTime);
                
                if (m_Time < desiredTime)
                    m_Time = desiredTime;
            }

            m_Director.time = m_Interpolation.Enabled ? m_Interpolation.Value.Evaluate((float)(m_Time / m_Duration)) * m_Duration : m_Time;
            m_Director.Evaluate();
        }

        IPlayableValueHandle IPlayableValue.Lock() => this;

        void IPlayableValueHandle.Set(float curveValue, float weight)
        {
            DesiredTime = weight;

#if UNITY_EDITOR
            if (enabled && Application.isPlaying == false)
            {
                var director = GetComponent<PlayableDirector>();
                director.time = director.duration * DesiredTime;
                director.Evaluate();
            }
#endif
        }

        void IPlayableValue.UnLock(IPlayableValueHandle handle)
        {
        }

        public void On()
        {
            SetDesiredTime(true);
        }

        public void Off()
        {
            SetDesiredTime(false);
        }
    }
}