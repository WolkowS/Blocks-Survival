using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayInterval : PlayNodeBase, IPlay
    {
        public Optional<AnimationCurve01> _lerp;
        [MinMaxSlider(0, 1)]
        public Vector2 _interval = new Vector2(0, 1);
        private IPlay _play;

        // =======================================================================
        protected override void _onPlay(float scale)
        {
            var time = (Mathf.Clamp(scale, _interval.x, _interval.y) - _interval.x) / (_interval.y - _interval.x);
            _invoke(_lerp.Enabled ? _lerp.Value.Evaluate(time) : time);
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && _scalePrev >= 0f)
            {
                _onPlay(_scalePrev);
            }
#endif
        }
    }
}