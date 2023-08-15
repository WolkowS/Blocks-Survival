using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CoreLib.SpriteAnimation
{
    public class SimpleSpriteAnimation : MonoBehaviour
    {
        public  TimeMode                       m_TimeMode;
        public  float                          m_FramesPerSec = 10;
        public  List<Sprite>                   m_Animation;
        public  bool                           m_Unscaled;
        public bool                            m_RandomOffset;
        private float                          m_StartTime;
        private SpriteAnimationPlayer.IAdapter m_Adapter;
        private float                          m_Seed;
        
        // =======================================================================
        [Serializable]
        public enum TimeMode
        {
            Individual,
            Discreet,
            Sync
        }

        // =======================================================================
        private void Start()
        {
            if (TryGetComponent<SpriteRenderer>(out var sr))
                m_Adapter = new SpriteAnimationPlayer.SpriteAdapter(sr);
            else
            if (TryGetComponent<Image>(out var im))
                m_Adapter = new SpriteAnimationPlayer.ImageAdapter(im);
        }

        private void OnEnable()
        {
            if (m_RandomOffset)
                m_Seed = Random.value * (m_Animation.Count / m_FramesPerSec);

            var time = m_Unscaled ? Time.unscaledTime : Time.time;
            m_StartTime = m_TimeMode switch
            {
                TimeMode.Individual => time,
                TimeMode.Discreet   => time - time % (1f / m_FramesPerSec),
                TimeMode.Sync       => time - time % (m_FramesPerSec),
                _                   => throw new ArgumentOutOfRangeException()
            };
        }

        private void Update()
        {
            var time  = (m_Unscaled ? Time.unscaledTime : Time.time) + m_Seed;
            var frame = Mathf.CeilToInt((time - m_StartTime) * m_FramesPerSec) % m_Animation.Count;
            m_Adapter.Set(m_Animation[frame]);
        }
    }
}