using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenReveal : Tween
    {
        public Optional<Vector2>  _scale;
        public Optional<Gradient> _color;

        private Transform    _target;
        private ColorAdapter _colorAdapter;

        // =======================================================================
        private void Awake()
        {
            _target = m_Root.GetValueOrDefault(gameObject).transform;
            if (_color.Enabled)
                _colorAdapter = new ColorAdapter(_target.gameObject);
        }
        
        public override void Apply()
        {
            var lerp = m_Input.Value;
            
            if (_scale.Enabled)
                _target.transform.localScale = Mathf.LerpUnclamped(_scale.Value.x, _scale.Value.y, lerp).ToVector3();
            
            if (_color.Enabled)
                _colorAdapter.Color = _color.Value.Evaluate(lerp);
        }

        public override void Revert()
        {
        }
    }
}