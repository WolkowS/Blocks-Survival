using System;
using Cinemachine;
using UnityEngine;

namespace CoreLib
{
    public class ColorImpulseListener : MonoBehaviour
    {
        public SpriteRenderer _sr;
        public Mode           _mode;
        public float          _gain = 1f;
        [CinemachineImpulseChannelProperty]
        public int            _channelMask;
        public Color          _weight = Color.white;
        public bool           _clamp;
        
        private Color _impact;
        private bool  _hasImpact;

        // =======================================================================
        public enum Mode
        {
            Magnitude,
            Colors,
            Alpha,
            Weight
        }

        // =======================================================================
        private void Update()
        {
            var haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(transform.position, true, _channelMask, out var impulsePos, out _);
            if (haveImpulse == false)
            {
                if (_hasImpact)
                    _clearImpact();
                
                return;
            }
            
            _hasImpact = true;
            Color impact;
            switch (_mode)
            {
                case Mode.Magnitude:
                    impact = _weight * _processInput(impulsePos.magnitude);
                    break;
                case Mode.Colors:
                    impact = _weight * new Color(_processInput(impulsePos.x), _processInput(impulsePos.y), _processInput(impulsePos.z), 1f);
                    break;
                case Mode.Alpha:
                    impact = _weight * new Color(1f, 1f, 1f, _processInput(impulsePos.magnitude));
                    break;
                case Mode.Weight:
                {
                    // normalize color
                    var c   = new Color(_processInput(impulsePos.x), _processInput(impulsePos.y), _processInput(impulsePos.z), 0f);
                    var max = Mathf.Max(c.r, c.g, c.b);
                    if (max > 0)
                    {
                        c   /= max;
                        c.a =  max;
                    }
                    impact =  _weight * c;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _sr.color += impact - _impact;
            _impact   =  impact;

            // -----------------------------------------------------------------------
            float _processInput(float val)
            {
                val *= _gain;
                
                if (_clamp == false)
                    return val;
                
                return Mathf.Clamp01(val);
            }
        }

        private void OnDisable()
        {
            _clearImpact();
        }

        private void _clearImpact()
        {
            _sr.color    -= _impact;
            _impact      =  Color.clear;
            _hasImpact   = false;
        }
    }
}