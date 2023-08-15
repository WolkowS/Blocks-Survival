using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Module
{
    public class TimeLayer : GvFloat
    {
        [SerializeField]
        private float m_TimeScaleInitial = 1f;
        public  float m_TimeScale;
        public  float m_TimeTotal;
        public  float m_DeltaTime;

        public Source m_Source;
        [ShowIf(nameof(m_Source), Source.Ticked)]
        public TickedQueue m_TickedQueue;
        [ShowIf(nameof(m_Source), Source.Ticked)]
        public float m_TickLength = 0.02f;

        // =======================================================================
        [Serializable]
        public enum Source
        {
            Unscaled,
            Fixed,
            Scaled,
            Ticked
        }

        private abstract class Updater : MonoBehaviour
        {
            [NonSerialized]
            public TimeLayer Owner;
        }

        private class UnscaledUpdater : Updater
        {
            private void Update()
            {
                Owner.Tick(Time.unscaledDeltaTime);
            }
        }

        private class FixedUpdater : Updater
        {
            private void FixedUpdate()
            {
                Owner.Tick(Time.fixedDeltaTime);
            }
        }

        private class TickedUpdater : Updater, ITicked, ITickedDelta
        {
            public IRefGet<float> TickedDelta { get; set; }
            float ITicked.        TickLength  => Owner.m_TickLength;

            void ITicked.OnTicked() => Owner.Tick(TickedDelta.Value);
        }

        private class ScaledUpdater : Updater
        {
            private void Update()
            {
                Owner.Tick(Time.deltaTime);
            }
        }

        // =======================================================================
        public override void Init()
        {
            base.Init();

            m_TimeScale = m_TimeScaleInitial;
            m_TimeTotal = 0;
            m_DeltaTime = 0;

            switch (m_Source)
            {
                case Source.Unscaled:
                    Core.Instance.gameObject.AddComponent<UnscaledUpdater>().Owner = this;
                    break;
                case Source.Fixed:
                    Core.Instance.gameObject.AddComponent<FixedUpdater>().Owner = this;
                    break;
                case Source.Scaled:
                    Core.Instance.gameObject.AddComponent<ScaledUpdater>().Owner = this;
                    break;
                case Source.Ticked:
                    Core.Instance.gameObject.AddComponent<TickedUpdater>().Owner = this;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void Tick(float deltaTime)
        {
            m_DeltaTime =  deltaTime;
            m_TimeTotal += deltaTime;

            m_Value = deltaTime;
            if (m_DeltaTime != 0f)
                _invokeOnChanged(m_DeltaTime);
        }
    }
}