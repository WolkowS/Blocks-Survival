using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib.Tween
{
    public class OscNoise : TweenOscillator
    {
        private float m_CurrentTime;
        public float m_Frequency = 1f;

        public override float Value
        {
            get
            {
                m_CurrentTime += DeltaTime * m_Frequency;
                m_Noise.GetSignal(m_CurrentTime, out var pos, out _);
                return pos.x;
            }
        }

        public override Vector2 Value2
        {
            get
            {
                m_CurrentTime += DeltaTime * m_Frequency;
                m_Noise.GetSignal(m_CurrentTime, out var pos, out _);
                return pos.To2DXY();
            }
        }

        public override Vector3 Value3
        {
            get
            {
                m_CurrentTime += DeltaTime * m_Frequency;
                m_Noise.GetSignal(m_CurrentTime, out var pos, out _);
                return pos;
            }
        }

        public float Frequency
        {
            get => m_Frequency;
            set => m_Frequency = value;
        }

        public global::Cinemachine.NoiseSettings m_Noise;

        private void Start()
        {
            m_CurrentTime = Random.Range(0, 1000f);
        }
    }
}