using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class Updater : MonoBehaviour, ITicked, ITickedDelta
    {
        [SerializeField]
        private Mode m_Mode;
        public event Action<float> OnUpdate;

        [SerializeField] [ShowIf(nameof(m_Mode), Mode.Ticked)]
        private TickedQueue m_TickedQueue;
        [SerializeField] [ShowIf(nameof(m_Mode), Mode.Ticked)]
        private float m_TickedInterval = TickedQueue.k_DefaultTickLength;

        private Component m_Updater;

        float ITicked.        TickLength  => m_TickedInterval;
        public IRefGet<float> TickedDelta { get; set; }

        // =======================================================================
        [Serializable]
        public enum Mode
        {
            Update,
            FixedUpdate,
            LateUpdate,
            Ticked,
            None
        }

        private class OnUpdateCaller : MonoBehaviour
        {
            public Action<float> OnUpdate;

            // =======================================================================
            private void Update()
            {
                OnUpdate.Invoke(Time.deltaTime);
            }
        }
        
        private class OnFixedUpdateCaller : MonoBehaviour
        {
            public Action<float> OnUpdate;

            // =======================================================================
            private void FixedUpdate()
            {
                OnUpdate.Invoke(Time.fixedDeltaTime);
            }
        }

        private class OnLateUpdateCaller : MonoBehaviour
        {
            public Action<float> OnUpdate;

            // =======================================================================
            private void LateUpdate()
            {
                OnUpdate.Invoke(Time.deltaTime);
            }
        }

        // =======================================================================
        public virtual void OnEnable()
        {
            switch (m_Mode)
            {
                case Mode.Update:
                {
                    var updater = gameObject.AddComponent<OnUpdateCaller>();
                    m_Updater        = updater;
                    updater.OnUpdate = OnUpdate;
                } break;
                case Mode.FixedUpdate:
                {
                    var updater = gameObject.AddComponent<OnFixedUpdateCaller>();
                    m_Updater        = updater;
                    updater.OnUpdate = OnUpdate;
                } break;
                case Mode.LateUpdate:
                {
                    var updater = gameObject.AddComponent<OnLateUpdateCaller>();
                    m_Updater        = updater;
                    updater.OnUpdate = OnUpdate;
                } break;
                case Mode.Ticked:
                {
                    m_TickedQueue.Add(this);
                } break;
                case Mode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void OnDisable()
        {
            switch (m_Mode)
            {
                case Mode.Update:
                case Mode.FixedUpdate:
                case Mode.LateUpdate:
                {
                    Destroy(m_Updater);
                } break;

                case Mode.Ticked:
                {
                    m_TickedQueue.Remove(this);
                } break;
            }

        }

        void ITicked.OnTicked()
        {
            OnUpdate.Invoke(TickedDelta.Value);
        }
    }
}