using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class Move : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _root;
        public Optional<Vers<GameObject>> _target;
        [HideIf(nameof(_target))]
        public Vector3 _direction;
        [HideIf(nameof(_target))]
        public bool _local;

        public Optional<Vers<float>> _speedMul;
        
        [HideIf(nameof(_motor))]
        public AddLerp _move;
        
        public Optional<Motor>  _motor;

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
        
        private void Update()
        {
            var root     = (_root.Enabled ? _root.Value.Value : gameObject).transform;
            var speedMul = _speedMul.Enabled ? _speedMul.Value.Value : 1f;
            var dir      = _target.Enabled ? _target.Value.Value.transform.position - root.position: (_local ? transform.rotation * _direction : _direction);
            
            root.transform.position = _motor.Enabled ?
                _motor.Value.Evaluate(root.transform.position, dir, Time.deltaTime * speedMul) :
                _move.Evaluate(transform.position, root.transform.position + dir, Time.deltaTime * speedMul);
        }
    }
}