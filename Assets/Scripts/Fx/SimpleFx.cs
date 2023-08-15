using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CoreLib;
using CoreLib.Cinemachine;
using UnityEngine;

namespace Game
{
    public class SimpleFx : Singleton<SimpleFx>
    {
        public CinemachineImpulseSource _recoil;
        public CinemachineImpulseSource _bump;
        public CinemachineImpulseSource _explosion;
        
        private List<ScaleImpact> _orthoImpact = new List<ScaleImpact>();
        private List<RollImpact>  _rollImpact  = new List<RollImpact>();

        private float _ortho;
        
        // =======================================================================
        public class ScaleImpact
        {
            public float _time;
            public float _scale;

            // =======================================================================
            public ScaleImpact(float time, float scale)
            {
                _time  = time;
                _scale = scale;
            }
        }
        
        public class RollImpact : IDisposable
        {
            public float _angle;
            public bool  _dispose;

            public void Dispose()
            {
                _dispose = true;
            }

            public RollImpact(float angle)
            {
                _angle    = angle;
            }
        }
        
        // =======================================================================
        public void AddOrthoImpact(float duration, float impact)
        {
            _orthoImpact.Add(new ScaleImpact(duration, impact));
        }
        
        public RollImpact AddRollImpact(float angle)
        {
            var rollImpact = new RollImpact(angle);
            _rollImpact.Add(rollImpact);
            
            return rollImpact;
        }

        private void Update()
        {
            _orthoImpact.RemoveAll(n => n._time <= 0f);
            
            var max    = 0f;
            var maxAbs = 0f;
            foreach (var impact in _orthoImpact)
            {
                impact._time -= Time.unscaledDeltaTime;
                var scaleAbs = impact._scale.Abs();
                if (maxAbs < scaleAbs)
                {
                    max = impact._scale;
                    maxAbs = scaleAbs;
                }
            }
            
            CmOrtho.s_Impact += max - _ortho;
            _ortho = max;
            
            _rollImpact.RemoveAll(n => n._dispose);
            
            var sum = _rollImpact.Sum(n => n._angle);
            CmRoll.s_Impact = sum;
        }
    }
}