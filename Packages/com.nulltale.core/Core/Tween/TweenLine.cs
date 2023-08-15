using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenLine : Tween
    {
        [MinMaxSlider(0, 1)]
        public Vector2 _size = new Vector2(0, 1);
        public  float  _offset           = -1f;
        public  float  _inputOffsetScale = 2f;
        private LineRenderer _lr;

        // =======================================================================
        private void Awake()
        {
            _lr = (m_Root.Enabled ? m_Root.Value : gameObject).GetComponent<LineRenderer>();
            Apply();
        }

        public override void Apply()
        {
            var offset = m_Input.Value * _inputOffsetScale;
            
            var start = (_size.x + _offset + offset).Clamp01();
            var end   = (_size.x + _offset + offset + _size.y).Clamp01();
            
            _lr.enabled = !(start == end && (start == 1f || start == 0f));
            if (_lr.enabled == false)
                return;

            var grad  = _lr.colorGradient;
            
            grad.SetKeys(grad.colorKeys,
                         new GradientAlphaKey[]
                         {
                             new GradientAlphaKey(start == 0f ? 1f : 0f, start),
                             new GradientAlphaKey(1, end),
                             new GradientAlphaKey(0f, 0f),
                             new GradientAlphaKey(0f, 1f),
                         });
            
            _lr.colorGradient = grad;
        }

        public override void Revert()
        {
        }
    }
}