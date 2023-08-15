using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CoreLib
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class ValueState : MonoBehaviour, IPlayableValue, IPlayableValueHandle, IToggle
    {
        public double _speed   = 1d;
        public double _upMul   = 1d;
        public double _downMul = 1d;
        
        public float  _upLerp;
        public float  _downLerp;

        public Optional<AnimationCurve> _interpolation;

        private double _time;
        public  bool   _unScaledTime = true;
        
        public  double _range        = 1d;

        [SerializeField] [Range(0, 1)]
        private double _desiredTime;

        public double DesiredTime
        {
            get => _desiredTime;
            set
            {
                var val = value.Clamp01();
                if (val == _desiredTime)
                    return;

                _desiredTime = val;
                enabled      = true;
            }
        }
        public bool IsOn => _desiredTime == 1d;

        public UnityEvent<float> _onChange;

        // =======================================================================
        public void SetDesiredTime(float normalizedTime)
        {
            DesiredTime = normalizedTime;
        }

        public void SetDesiredTime(bool time)
        {
            DesiredTime = time ? 1d : 0d;
        }

        private void Awake()
        {
            _time = _range * DesiredTime;
        }

        private void OnEnable()
        {
            _invoke();
        }

        private void OnValidate()
        {
            if (_range * DesiredTime != _time)
                enabled = true;
        }

        private void Update()
        {
            var desiredTime = _range * DesiredTime;

            if (_time == desiredTime)
            {
                enabled = false;
                return;
            }

            var deltaTime = _unScaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (desiredTime > _time)
            {
                _time += _upMul * _speed * deltaTime;
                _time =  Mathf.Lerp((float)_time, (float)desiredTime, _upLerp * deltaTime);
                
                if (_time > desiredTime)
                    _time = desiredTime;
            }
            else
            {
                _time -= _downMul * _speed * deltaTime;
                _time =  Mathf.Lerp((float)_time, (float)desiredTime, _downLerp * deltaTime);
                
                if (_time < desiredTime)
                    _time = desiredTime;
            }

            _invoke();
        }
        
        private void _invoke()
        {
            _onChange.Invoke((float)(_interpolation.Enabled ? _interpolation.Value.Evaluate((float)(_time / _range)) * _range : _time));
        }

        IPlayableValueHandle IPlayableValue.Lock() => this;

        void IPlayableValueHandle.Set(float curveValue, float weight)
        {
            DesiredTime = weight;

#if UNITY_EDITOR
            if (enabled && Application.isPlaying == false)
            {
                var director = GetComponent<PlayableDirector>();
                director.time = director.duration * DesiredTime;
                director.Evaluate();
            }
#endif
        }

        void IPlayableValue.UnLock(IPlayableValueHandle handle)
        {
        }

        public void On()
        {
            SetDesiredTime(true);
        }

        public void Off()
        {
            SetDesiredTime(false);
        }
    }
}