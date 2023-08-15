using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayRepeat : PlayNodeBase, IPlay
    {
        public Optional<AnimationCurve01> _lerp;
        
        public  float _repeat = 1;
        public  float _offset;
        private IPlay _play;

        // =======================================================================
        protected override void _onPlay(float scale)
        {
            var time = scale * _repeat + _offset;
            _invoke(_lerp.Enabled ? _lerp.Value.Evaluate(time) : time);
        }
    }
}