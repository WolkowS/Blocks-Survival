using UnityEngine;

namespace CoreLib.Play
{
    public class PlayReveal : PlayBase
    {
        public  Optional<Vector2>  _scale = new Optional<Vector2>(new Vector2(0, 1));
        public  Optional<Gradient> _color;
        private ColorAdapter       _adapter;
        
        // =======================================================================
        private void Awake()
        {
            _adapter = new ColorAdapter(gameObject);
        }

        protected override void _onPlay(float scale)
        {
            if (_color.Enabled)
                _adapter.Color = _color.Value.Evaluate(scale);
            
            if (_scale.Enabled)
            {
                var range = _scale.Value;
                transform.localScale = Mathf.LerpUnclamped(range.x, range.y, (_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale)).ToVector3();
            }
        }
    }
}