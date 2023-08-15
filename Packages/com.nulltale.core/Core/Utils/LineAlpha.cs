using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class LineAlpha : MonoBehaviour
    {
        [MinMaxSlider(0, 1)]
        public Vector2 _range = new Vector2(0, 1);

        private LineRenderer _lr;

        // =======================================================================
        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            var grad = _lr.colorGradient;
            grad.SetKeys(grad.colorKeys,
                         new GradientAlphaKey[]
                         {
                             new GradientAlphaKey(_range.x == 0f ? 1f : 0f, _range.x),
                             new GradientAlphaKey(1, _range.y),
                             new GradientAlphaKey(0f, 0f),
                             new GradientAlphaKey(0f, 1f),
                         });
            _lr.colorGradient = grad;
        }
    }
}