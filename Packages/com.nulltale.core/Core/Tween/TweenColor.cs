using System;
using CoreLib.Fx;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Tween
{
    public class TweenColor : Tween
    {
        public ColorMode m_Evaluation = ColorMode.Gradient;
        public SetMode   m_Set        = SetMode.Apply;
        [ShowIf(nameof(m_Evaluation), ColorMode.Color)] [Label("Color")]
        public  Color         m_Tint = Color.white;
        [ShowIf(nameof(m_Evaluation), ColorMode.Gradient)]
        public  Gradient      m_Gradient;
        private Color         m_Impact;
        private ColorAdapter  m_ColorAdapter;
        
        public Color Tint
        {
            get => m_Tint;
            set => m_Tint = value;
        }

        // =======================================================================
        public enum ColorMode
        {
            Color,
            Alpha,
            Gradient
        }

        public enum SetMode
        {
            Impact,
            Apply,
        }

        // =======================================================================
        private void Awake()
        {
            m_ColorAdapter = new ColorAdapter(m_Root.GetValueOrDefault(gameObject));
        }

        public override void Apply()
        {
            var impact = m_Evaluation switch
            {
                ColorMode.Color    => (m_Input.Value3 * Weight).ToColor() * m_Tint,
                ColorMode.Alpha    => m_Set == SetMode.Impact ? new Color(0, 0, 0, m_Input.Value * Weight) : m_ColorAdapter.Color.WithA(m_Input.Value * Weight),
                ColorMode.Gradient => m_Gradient.Evaluate(m_Input.Value) * Weight,
                _                  => throw new ArgumentOutOfRangeException()
            };
                
            if (impact == m_Impact)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying == false && m_ColorAdapter == null)
                Awake();
#endif

            switch (m_Set)
            {
                case SetMode.Impact:
                    m_ColorAdapter.Color = m_ColorAdapter.Color + impact - m_Impact;
                    break;
                case SetMode.Apply:
                    m_ColorAdapter.Color = impact;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            m_Impact = impact;
        }

        public override void Revert()
        {
            m_ColorAdapter.Color -= m_Impact;
            m_Impact             =  Color.clear;
        }
    }
}