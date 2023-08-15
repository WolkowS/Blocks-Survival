using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace CoreLib.Scripting
{
    public class Remap : MonoBehaviour
    {
        public Vector2 _inRange;
        public Vector2 _outRange;
        public Mode    _mode = Mode.Clamp;
        
        public UnityEvent<float> _onInvoke;

        // =======================================================================
        public enum Mode
        {
            None,
            Clamp,
            Loop,
            PingPong,
        }
        
        // =======================================================================
        public void Invoke(int val)
        {
            Invoke((float)val);
        }
        
        public void Invoke(float val)
        {
            var inRange  = _inRange.y - _inRange.x;
            var outRange = _outRange.y - _outRange.x;
            
            var scale = (val - _inRange.x) / inRange;

            switch (_mode)
            {
                case Mode.None:
                    break;
                case Mode.Clamp:
                {
                    scale = Mathf.Clamp01(scale);
                } break;
                case Mode.Loop:
                {
                    scale = scale % 1f;
                    if (scale < 0f)
                        scale = 1f - scale;
                } break;
                case Mode.PingPong:
                default:
                    throw new ArgumentOutOfRangeException();
            }
              
            var result = scale * outRange + _outRange.x;
            _onInvoke.Invoke(result);
            
        }
    }
}