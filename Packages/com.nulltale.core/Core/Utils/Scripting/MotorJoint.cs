using UnityEngine;

namespace CoreLib.Scripting
{
    public class MotorJoint : Move.Motor
    {
        public  float           _accel;
        public  float           _damping;
        public  Optional<float> _maxDist;
        private Vector3         _vel;
        
        // =======================================================================
        public override Vector3 Evaluate(Vector3 pos, Vector3 dir, float deltaTime)
        {
            _vel -= _vel * (_damping * deltaTime);
            _vel += dir * (_accel * dir.magnitude * deltaTime);
            if (_maxDist.Enabled)
            {
                var goal = pos + dir;
                if (dir.magnitude >= _maxDist.Value)
                    pos += dir - dir.ClampMagnitude(_maxDist.Value);
            }

            return pos + _vel * deltaTime;
        }
    }
}