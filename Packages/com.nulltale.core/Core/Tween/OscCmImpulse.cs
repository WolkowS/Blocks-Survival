using System;
using Cinemachine;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscCmImpulse : TweenOscillator, IToggle
    {
        public bool            m_IsOn;
        public Vector2         m_Range = new Vector2(0, 1);
        public float           m_Gain   = 1f;
        [CinemachineImpulseChannelProperty]
        public int             m_ChannelMask;

        private float   m_Scale;
        private Vector3 m_Value;

        public override float Value
        {
            get
            {
                _getValue();
                return m_Value.x * m_Gain;
            }
        }

        public override Vector2 Value2
        {
            get
            {
                _getValue();
                return m_Value.To2DXY() * m_Gain;
            }
        }

        public override Vector3 Value3
        {
            get
            {
                _getValue();
                return m_Value * m_Gain;
            }
        }

        public bool IsOn
        {
            get => m_IsOn;
            set => m_IsOn = value;
        }

        public void On()  => IsOn = true;
        public void Off() => IsOn = false;
        
        // =======================================================================
        private void _getValue()
        {
            var haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(transform.position, true, m_ChannelMask, out m_Value, out _);
            if (haveImpulse == false)
                m_Value = Vector3.zero;
        }
    }
}