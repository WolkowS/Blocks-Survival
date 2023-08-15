using System;
using Cinemachine;
using UnityEngine;

namespace CoreLib.Tween
{
    public class ValueImpulse : OscillatorBase
    {
        [CinemachineImpulseChannelProperty]
        public int _channelMask;

        public  float   _gain = 1f;
        public  Mode    _mode;
        private Vector3 _value;
        
        public override float   Value
        {
            get
            {
                _update();
                return _value.x;
            }
        }

        public override Vector2 Value2
        {
            get
            {
                _update();
                return _value.To2DXY();
            }
        }

        public override Vector3 Value3
        {
            get
            {
                _update();
                return _value;
            }
        }

        // =======================================================================
        public enum Mode
        {
            Magnitude,
            Vector,
        }

        // =======================================================================
        private void _update()
        {
            var haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(transform.position, true, _channelMask, out var impulsePos, out _);
            if (haveImpulse == false)
            {
                _value = Vector3.zero;
                return;
            }

            switch (_mode)
            {
                case Mode.Magnitude:
                {
                    var mag = impulsePos.magnitude * _gain;
                    _value = new Vector3(mag, mag, mag);
                } break;
                
                case Mode.Vector:
                {
                    _value = impulsePos * _gain;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}