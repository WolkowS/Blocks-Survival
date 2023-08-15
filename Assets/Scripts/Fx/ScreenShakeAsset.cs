using System;
using Cinemachine;
using CoreLib;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class ScreenShakeAsset : ScriptableObject
    {
        public Optional<Vector2> _velocity;
        public float             _force;
        public Mode              _mode;
        
        // =======================================================================
        public enum Mode
        {
            Recoil,
            Explosion,
            Bump
        }
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            var impulseSource = _impulseSource();
            var vel           = (_velocity.Enabled ? _velocity.Value : UnityRandom.Normal2D()) * impulseSource.m_DefaultVelocity.magnitude;
            
            impulseSource.GenerateImpulse((vel * _force).To3DXY());
        }

        private CinemachineImpulseSource _impulseSource()
        {
            var impulseSource = _mode switch
            {
                Mode.Recoil    => SimpleFx.Instance._recoil,
                Mode.Explosion => SimpleFx.Instance._explosion,
                Mode.Bump      => SimpleFx.Instance._bump,
                _              => throw new ArgumentOutOfRangeException()
            };
            return impulseSource;
        }

        /*public void Invoke(Vector2 pos)
        {
            var impulseSource = _impulseSource();
            
            var vel = (Player.Instance._rb.position - pos).normalized;
            if (_velocity.Enabled)
                vel = _velocity.Value.Rotate(vel.AngleRad());
            
            impulseSource.GenerateImpulse((vel * impulseSource.m_DefaultVelocity.magnitude * _force).To3DXY());
        }*/
    }
}