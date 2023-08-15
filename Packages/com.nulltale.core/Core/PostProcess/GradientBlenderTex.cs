using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class GradientBlenderTex : MonoBehaviour
    {
        [Range(0, 1)]
        public float _weight;
        private float _weightPrev;
        public Gradient _gradient;
        [ReadOnly]
        public Texture2D _tex;

        private GradientBlender _owner;

        public float Weight
        {
            get => _weight;
            set
            {
                if (_weight == value)
                    return;
                
                _owner.enabled = true;
                
                _weight = value;
            }
        }

        // =======================================================================
        private void Awake()
        {
            _owner = GetComponentInParent<GradientBlender>();
            _rebuild();
        }

        private void OnEnable()
        {
            _owner._grads.Add(this);
            _owner.enabled = true;
        }

        private void OnDisable()
        {
            _owner._grads.Remove(this);
        }

        public void Require() => _rebuild();
        
        private void OnValidate()
        {
            if (_owner)
                _owner.enabled = true;
            
#if UNITY_EDITOR
            if (UnityEditor.Selection.Contains(gameObject))
                _rebuild();
#endif
        }

        // =======================================================================
        private void _rebuild()
        {
            if (_weight == _weightPrev)
                return;
            
            _tex = new Texture2D(_owner.Width, 1, TextureFormat.RGBA32, false);
            for (var x = 0; x < _owner.Width; x++)
                _tex.SetPixel(x, 0, _gradient.Evaluate(x / (float)(_owner.Width - 1)));
            
            _tex.Apply(false, false);
            _weightPrev = _weight;
        }
    }
}