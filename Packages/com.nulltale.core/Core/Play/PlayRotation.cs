using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayRotation : PlayBase
    {
        public float   _offset;
        public Vector2 _range;
        
        // =======================================================================
        protected override void _onPlay(float scale)
        {
            scale += _offset;
            var angle = Mathf.LerpUnclamped(_range.x, _range.y, (_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale));
            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.back);
        }
    }
}