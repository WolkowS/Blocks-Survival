using UnityEngine;

namespace CoreLib.Scripting
{
    public class MotorVelocity : Move.Motor
    {
        public  float           _accel;
        public  Optional<float> _maxVel;
        private Vector3         _vel;
        
        // =======================================================================
        public override Vector3 Evaluate(Vector3 pos, Vector3 dir, float deltaTime)
        {
            _vel = Vector3.MoveTowards(_vel, dir, _accel * deltaTime);
            
            if (_maxVel.Enabled)
                _vel = _vel.ClampMagnitude(_maxVel.Value);
            
            return pos + _vel * deltaTime;
        }
    }
}