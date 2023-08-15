using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(TimeControl), menuName = Core.k_CoreModuleMenu + nameof(TimeControl))]
    public class TimeControl : Core.Module<TimeControl>
    {
        private static ObjectPool<TimeHandle> s_TimeHandlesPool;

        public const float                  k_MinFixedDeltaTime       = 0.001f;
        public const float                  k_MaxFixedDeltaTime       = 0.100f;
        public const float                  k_MaxTimeScale            = 100.0f;
        public const int                    k_SlowDownDefaultPriority = 0;
        public const int                    k_SpeedUpDefaultPriority  = 100;
        public const int                    k_InstantDefaultPriority  = -100;

        private static  Ref<float> s_FixedDeltaTimeRef = new Ref<float>();
        private static Ref<float> s_DeltaTimeRef      = new Ref<float>();

        public static IRefGet<float> FixedDeltaTimeRef => s_FixedDeltaTimeRef;
        public static IRefGet<float> DeltaTimeRef      => s_DeltaTimeRef;

        [SerializeField]
        private bool					m_CreateTimeController;
        [SerializeField, ShowIf(nameof(m_CreateTimeController)), Tooltip("Num +, Num -")]
        private bool					m_EnableNumpadControl;

        [SerializeField, Range(0.0f, 10.0f)]
        private float                   m_InitialGameSpeed = 1.0f;
        private float                   m_InitialFixedDeltaTime;
        private float                   m_GameSpeed = 1.0f;
        [SerializeField]
        private bool                    m_IgnoreEffects;

        [SerializeField]
        private float                   m_TransitionTimeDefault = 0.1f;
        [SerializeField] [CurveRange(0, 0, 1, 1)]
        private AnimationCurve          m_TransitionCurveDefault = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        private bool                    m_ScaleFixedDeltaTime;

        [SerializeField]
        private SoCollection<TimeLayer> m_TimeLayers;
        
        private TimeHandle  m_DefaultHandle;
        private TimeHandle  m_DesiredHandle;
        private float       m_TimeTransition;
        private float       m_ScaleTransition;

        private Switch        m_PauseLock;
        private TimeHandle  m_Pause;

        [ShowNativeProperty]
        private float                   TimeScale
        {
            get => Time.timeScale;
            set
            {
                Time.timeScale = value;
                    
                // apply physics scale if game speed not zero
                if (m_ScaleFixedDeltaTime)
                    Time.fixedDeltaTime = Mathf.Clamp(Instance.m_InitialFixedDeltaTime * value, k_MinFixedDeltaTime, k_MaxFixedDeltaTime);
            }
        }

        public bool IgnoreEffects
        {
            get => m_IgnoreEffects;
            set => m_IgnoreEffects = value;
        }

        private SortedCollection<TimeHandle> m_TimeHandles;

        // =======================================================================
        public class TimeHandle : IDisposable
        {
            public  int            Priority { get; internal set; }
            public  BlendingMode   Blending { get; set; }
            private float          m_Scale;
            public  AnimationCurve TransitionCurve { get; set; }
            public  float          TransitionTime  { get; set; }
            private bool           m_IsOpen;
            private bool           m_Disposed;

            public float Scale
            {
                get => m_Scale;
                set => m_Scale = Mathf.Clamp(value, 0f, k_MaxTimeScale);
            }

            // =======================================================================
            [Serializable]
            public enum BlendingMode
            {
                Floor,
                Ceil,
                Override
            }

            // =======================================================================
            public void Open()
            {
                if (m_Disposed)
                    throw new AccessViolationException("Can't use disposed time handle");

                if (m_IsOpen)
                    return;
                
                m_IsOpen = true;
                Instance.m_TimeHandles.Add(this);
            }

            public void Close()
            {
                if (m_IsOpen == false)
                    return;
                
                m_IsOpen = false;
                Instance.m_TimeHandles.Remove(this);
            }

            public void Dispose()
            {
                if (m_Disposed)
                    return;

                Close();
                m_Disposed = true;
                s_TimeHandlesPool.Release(this);
            }

            public static TimeHandle Create(float scale, BlendingMode blending)
            {
                return Create(scale, blending, blending switch
                {
                    BlendingMode.Floor    => k_SlowDownDefaultPriority,
                    BlendingMode.Ceil    => k_SpeedUpDefaultPriority,
                    BlendingMode.Override => k_InstantDefaultPriority,
                    _                           => throw new ArgumentOutOfRangeException(nameof(blending), blending, null)
                });
            }

            public static TimeHandle Create(float scale, BlendingMode blending, int priority)
            {
                var handle = s_TimeHandlesPool.Get();
                handle.m_Disposed = false;
                handle.Scale    = scale;
                handle.Blending = blending;
                handle.Priority = priority;

                return handle;
            }
        }

        [Serializable]
        public class SlowMo
        {
            public float    Duration;
            [Range(0, 1)]
            public float    Scale;

            public void Invoke()
            {
                SlowDown(Scale, Duration);
            }
        }

        [DefaultExecutionOrder(-1000)]
        private class TimeUpdater : MonoBehaviour
        {
            private void Awake()
            {
                hideFlags = HideFlags.HideAndDontSave;
            }

            private void Update()
            {
                s_DeltaTimeRef.Value      = Time.deltaTime;
                s_FixedDeltaTimeRef.Value = Time.fixedDeltaTime;
            }

            private void LateUpdate()
            {
                Instance._updateTimeScale();
            }
        }

        // =======================================================================
        public override void Init()
        {
            m_TimeHandles           = new SortedCollection<TimeHandle>(Comparer<TimeHandle>.Create((a, b) => a.Priority - b.Priority));
            m_InitialFixedDeltaTime = Time.fixedDeltaTime;

            s_TimeHandlesPool = new ObjectPool<TimeHandle>(null, handle =>
            {
                handle.TransitionTime  = Instance.m_TransitionTimeDefault;
                handle.TransitionCurve = Instance.m_TransitionCurveDefault;
            }, false);

            m_DefaultHandle = TimeHandle.Create(1f, TimeHandle.BlendingMode.Override, int.MinValue);
            m_DesiredHandle = m_DefaultHandle;
            
            // apply time scale
            SetGameSpeed(m_InitialGameSpeed);

            // create updater
            Core.Instance.gameObject.AddComponent<TimeUpdater>();

            // init layers
            foreach (var timeLayer in m_TimeLayers)
                timeLayer.Init();

            m_PauseLock = new Switch();
            m_Pause = TimeHandle.Create(0f, TimeHandle.BlendingMode.Override, 100000);

#if UNITY_EDITOR
            // create controller
            if (m_CreateTimeController)
            {
                var timeController = Core.Instance.gameObject.AddComponent<TimeController>();
                timeController.Init(m_InitialGameSpeed, m_EnableNumpadControl);
            }
#endif
        }
        
        // =======================================================================
        public static void SetGameSpeed(float gameSpeed)
        {
            Instance.m_GameSpeed = gameSpeed;
        }

        public static void SlowDown(float timeScale, float duration)
        {
            if (duration <= .0f)
                return;

            var handle = TimeHandle.Create(timeScale, TimeHandle.BlendingMode.Floor, k_SlowDownDefaultPriority);
            Core.Instance.StartCoroutine(_waitTimeHandleUnscaled(duration, handle));
        }

        public static void Instant(float timeScale, float duration)
        {
            if (duration <= 0.0f)
                return;

            var handle = TimeHandle.Create(timeScale, TimeHandle.BlendingMode.Override, k_InstantDefaultPriority);
            Core.Instance.StartCoroutine(_waitTimeHandleUnscaled(duration, handle));
        }
        
        public static void SpeedUp(float timeScale, float duration)
        {
            if (duration <= 0.0f)
                return;

            var handle = TimeHandle.Create(timeScale, TimeHandle.BlendingMode.Ceil, k_SpeedUpDefaultPriority);
            Core.Instance.StartCoroutine(_waitTimeHandleUnscaled(duration, handle));
        }

        public static void Pause(bool on)
        {
            if (Instance.m_PauseLock.Apply(on))
            {
                if (Instance.m_PauseLock.IsOn)
                    Instance.m_Pause.Open();
                else
                    Instance.m_Pause.Close();
            }
        }

        // =======================================================================
        private static IEnumerator _waitTimeHandleUnscaled(float time, TimeHandle handle)
        {
            handle.Open();
            yield return new WaitForSecondsRealtime(time);
            handle.Dispose();
        }

        private void _updateTimeScale()
        {
            // update game speed
            if (m_IgnoreEffects)
            {
                TimeScale = m_GameSpeed;
                return;
            }

            // select desired handle
            var handle = m_DefaultHandle;
            foreach (var timeHandle in m_TimeHandles)
            {
                switch (timeHandle.Blending)
                {
                    case TimeHandle.BlendingMode.Floor:
                        if (handle.Scale > timeHandle.Scale)
                            handle = timeHandle;
                        break;
                    case TimeHandle.BlendingMode.Ceil:
                        if (handle.Scale < timeHandle.Scale)
                            handle = timeHandle;
                        break;
                    case TimeHandle.BlendingMode.Override:
                            handle = timeHandle;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                };
            }

            if (m_DesiredHandle != handle)
            {
                m_ScaleTransition = m_DesiredHandle.Scale;
                m_TimeTransition  = Time.unscaledTime;

                m_DesiredHandle = handle;
            }

            var scale = m_DesiredHandle.TransitionTime <= .0f ? m_DesiredHandle.Scale : 
                Mathf.Lerp(m_ScaleTransition, m_DesiredHandle.Scale, 1f - (m_TimeTransition + m_DesiredHandle.TransitionTime - Time.unscaledTime) / m_DesiredHandle.TransitionTime);
            TimeScale = scale * m_GameSpeed;
        }
    }
}