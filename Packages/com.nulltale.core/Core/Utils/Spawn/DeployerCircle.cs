using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerCircle : Deployer
    {
        public  Mode  _mode;
        [ShowIf(nameof(_mode), Mode.Segments)]
        public  int   _segments;
        [ShowIf(nameof(_mode), Mode.Step)]
        public  float _stepDeg;
        private float _angle;

        // =======================================================================
        public enum Mode
        {
            Segments,
            Step
        }
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            pos    =  transform.position;
            rot    =  transform.rotation * Quaternion.AngleAxis(_angle, Vector3.forward);
            var step = _mode switch
            {
                Mode.Segments => 360f / _segments,
                Mode.Step     => _stepDeg,
                _             => throw new ArgumentOutOfRangeException()
            };
            _angle += step;
        }
    }
}