using System;
using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class RectMimic : MonoBehaviour
    {
        public RectTransform _target;
        
        public bool _width = true;
        public bool _height = true;
        public Vector2 _padding;

        private RectTransform _this;
        
        // =======================================================================
        private void Awake()
        {
            _this = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            var deltaSize = _this.sizeDelta;
            
            if (_width)
                deltaSize.x = _target.sizeDelta.x + _padding.x;
            if (_height)
                deltaSize.y = _target.sizeDelta.y + _padding.y;
            
            _this.sizeDelta = deltaSize;
        }
    }
}