using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayScale : PlayBase
    {
        public Optional<Transform> _target;
        public Vector2             _range;
        public Optional<Mode>      _mode;
        
        // =======================================================================
        [Flags]
        public enum Mode
        {
            None = 0,
            X = 1,
            Y = 1 << 1,
            Z = 1 << 2,
            
            XY = X | Y,
            XYZ = -1,
        }
        
        // =======================================================================
        protected override void _onPlay(float scale)
        {
            var localScale = Mathf.LerpUnclamped(_range.x, _range.y, (_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale)).ToVector3();
            var trans = _target.GetValueOrDefault(transform);
            
            if (_mode.Enabled)
            {
                var mode = _mode.Value;
                if ((mode & Mode.X) == Mode.None)
                    localScale.x = trans.localScale.x;
                
                if ((mode & Mode.Y) == Mode.None)
                    localScale.y = trans.localScale.y;
                
                if ((mode & Mode.Z) == Mode.None)
                    localScale.z = trans.localScale.z;
            }
            
            trans.localScale = localScale;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && _scalePrev >= 0f)
            {
                var scale = _scalePrev;
                _scalePrev = -1;
                _onPlay(scale);
            }
#endif
        }
    }
}