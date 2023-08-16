using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using CoreLib.Tween;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Platform : MonoBehaviour
    {
        public GameObject _moveDir;
        public Motor      _motor;

        public KinematicController2D _controller;  

        public ParticleSystem.MinMaxCurve _waitTime;
        
        public float _horizontalWeight = 1f;
        public float _verticalWeight   = 1f;
        public float _directionOffset  = .3f;
        
        public bool _canLeft  = true;
        public bool _canRight = true;
        public bool _canUp    = true;
        public bool _canDown  = true;
        
        public int _spawnWeight;
        
        public GameObject _spawn;
        
        private bool              _waitChoose;
        public  UnityEvent<float> _waitMove;
        private Vector2           _moveVector;
        public UnityEvent         _onHit;
        //public UnityEvent _onStun;
        // public UnityEvent _onMove;
        // public UnityEvent _onCantMove;

        // =======================================================================
        [Serializable]
        public class Motor
        {
            [NonSerialized]
            public Vector2 _direction;
            [Label("Speed")]
            public ParticleSystem.MinMaxCurve _speedRoll;
            [NonSerialized]
            public float   _speed;
            [CurveRange]
            public AnimationCurve _acceleration;
            [NonSerialized]
            public float _time;
            
            // =======================================================================
            public void Stop()
            {
                _time = 0f;
                _direction = Vector2.zero;;
            }

            public void Run(Vector2 dir)
            {
                _direction = dir;
                _speed = _speedRoll.Evaluate();
            }
            
            public Vector2 Tick(float delta)
            {
                var offset = _direction * _speed * delta * _acceleration.Evaluate(_time) * GamePrefs.Instance._platformSpeedMul.Value;
                _time += delta;
                return offset;
            }
        }
        // =======================================================================
        public bool CanLeft
        {
            get => _canLeft;
            set => _canLeft = value;
        }

        public bool CanRight
        {
            get => _canRight;
            set => _canRight = value;
        }

        public bool CanUp
        {
            get => _canUp;
            set => _canUp = value;
        }

        public bool CanDown
        {
            get => _canDown;
            set => _canDown = value;
        }

        public void AddSpawnWeight(int weight)
        {
            _spawnWeight += weight;
        }

        private void Update()
        {
            _controller.Move(_motor.Tick(Time.deltaTime));
            
            if (_controller.Velocity.magnitude >= 0.0001f)
                return;
            
            if (_waitChoose)
                return;
            
            _motor.Stop();
            _chooseDelayed();
        }

        // =======================================================================
        [Button]
        public void Move()
        {
            StopAllCoroutines();
            _motor.Stop();
            _chooseDelayed();
        }
        
        [Button]
        public void Hit()
        {
            Immediate();
            _onHit.Invoke();
        }
        
        [Button]
        public void Immediate()
        {
            StopAllCoroutines();
            _chooseDir();
        }
        
        private void _chooseDelayed()
        {
            _waitChoose = true;
            var time = _waitTime.Evaluate() * GamePrefs.Instance._platformCooldownMul.Value;
            _waitMove.Invoke(time);
            _moveDir.transform.localPosition = Vector3.zero;
            this.Delayed(_chooseDir, time);
        }

        private void _chooseDir()
        {
            var dir = _getDirs()
                      .Except(_moveVector.ToDirection())
                      .ToWeightedBag(n => n.IsVertical() ? _verticalWeight : _horizontalWeight)
                      .NextOrDefault();

            _moveVector = dir switch
            {
                Direction.None  => Vector2.zero,
                Direction.Left  => Vector2.left,
                Direction.Right => Vector2.right,
                Direction.Up    => Vector2.up,
                Direction.Down  => Vector2.down,
                _               => throw new ArgumentOutOfRangeException()
            };
            
            _moveDir.transform.localPosition = _moveVector.To3DXY() * _directionOffset;
            _motor.Run(_moveVector);
            
            _waitChoose = false;
            if (_moveVector == Vector2.zero)
                _chooseDelayed();
        }

        private IEnumerable<Direction> _getDirs()
        {
            if (CanLeft)
                yield return Direction.Left;
            if (CanRight)
                yield return Direction.Right;
            if (CanUp)
                yield return Direction.Up;
            if (CanDown)
                yield return Direction.Down;
        }
    }
}