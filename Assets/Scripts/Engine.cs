using CoreLib;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class Engine : MonoBehaviour
    {
        public KinematicController2D        _root;
        public Optional<Vers<GameObject>> _target;
        [HideIf(nameof(_target))]
        public Vector2 _direction;
        [HideIf(nameof(_target))]
        public bool _local;

        public Optional<Vers<float>> _speedMul;
        
        [HideIf(nameof(_motor))]
        public AddLerp _move;
        
        public Optional<Motor> _motor;

        public float SpeedMul
        {
            get => _speedMul.Value.Value;
            set => _speedMul.Value.Value = value;
        }
        // =======================================================================
        public abstract class Motor : MonoBehaviour
        {
            public abstract Vector3 Evaluate(Vector3 pos, Vector3 dir, float deltaTime);
        }

        // =======================================================================
        public void SetDirection(Vector2 dir)
        {
            _direction = dir;
        }
        
        public void SetDirection(Vector3 dir)
        {
            _direction = dir;
        }
        
        public void SetTarget(GameObject go)
        {
            if (_target.Enabled && _target.Value.IsRef == false)
            {
                _target.Value.Value = go;
            }
            else
            {
                _direction = transform.position.DirTo(go.transform.position).normalized * _direction.magnitude;
            }
        }
        
        private void FixedUpdate()
        {
            var speedMul = _speedMul.Enabled ? _speedMul.Value.Value : 1f;
            var dir      = _target.Enabled ? _target.Value.Value.transform.position.To2DXY() - _root.transform.position.To2DXY(): (_local ? transform.rotation * _direction : _direction);
            
            _root.Move((_motor.Enabled
                           ? _motor.Value.Evaluate(_root.transform.position, dir, Time.deltaTime * speedMul)
                           : _move.Evaluate(transform.position, _root.transform.position + dir.To3DXY(), Time.deltaTime * speedMul)) - _root.transform.position);
        }
    }
}