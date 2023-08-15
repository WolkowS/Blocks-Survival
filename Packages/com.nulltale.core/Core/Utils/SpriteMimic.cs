using System;
using UnityEngine;

namespace CoreLib
{
    [RequireComponent(typeof(SpriteRenderer))][ExecuteAlways]
    public class SpriteMimic : MonoBehaviour
    {
        public  SpriteRenderer _target;
        public  ColorMode      _colorMode;
        public  int            _orderOffset;
        private SpriteRenderer _sprite;

        // =======================================================================
        public enum ColorMode
        {
            None,
            Copy,
            NoAlpha,
        }
        
        // =======================================================================
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            _sprite.enabled = _target.enabled && _target.gameObject.activeInHierarchy;
            if (_sprite.enabled == false)
                return;
            
            _sprite.sprite       = _target.sprite;
            if (_colorMode != ColorMode.None)
            {
                _sprite.color = _colorMode switch
                {
                    ColorMode.Copy                => _target.color,
                    ColorMode.NoAlpha             => _target.color.WithA(_sprite.color.a),
                    _                             => throw new ArgumentOutOfRangeException()
                };
            }
            _sprite.sortingOrder = _target.sortingOrder + _orderOffset;
        }
    }
}