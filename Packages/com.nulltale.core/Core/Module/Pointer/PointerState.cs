using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Module
{
    public class PointerState : ScriptableObject
    {
        public  PointerId        _id;
        public  Optional<Texture2D>       _spriteSheet = new Optional<Texture2D>(null, true);
        [ShowIf(nameof(_spriteSheet))]
        public  Vector2         _hotspot;
        [ShowIf(nameof(_spriteSheet))]
        public  Optional<float> _frameRate = new Optional<float>(1f, false);
        //[ShowIf(nameof(_frameRate))]
        //public  Optional<Vector2Int> _loopFrames;
        private bool        _visible;
        private Texture2D[] _sprites;
        private float       _time;
        private Texture2D   _current;
        private Vector2     _realHotspot;

        // =======================================================================
        public void Assign()
        {
            _time    = Time.unscaledTime;
            _current = null;
            
            Cursor.visible = _visible;
            
            _id.SetActive(true);
        }
        
        public void Release()
        {
            _id.SetActive(false);
        }
        
        public void Update()
        {
            if (_visible == false)
                return;
            
            var delta = Time.unscaledTime - _time;

            var index = _frameRate.Enabled ? (delta / (1f / _frameRate.Value)).FloorToInt() % _sprites.Length : 0;
            var current = _sprites[index];
            if (_current == current)
                return;
            _current = current;
            
            Cursor.SetCursor(_current, _realHotspot, CursorMode.Auto);
        }
        
        public void Init()
        {
            _visible  = _spriteSheet.Enabled;
            if (_visible == false)
                return;
            
            var spriteSheet = _spriteSheet.Value;
            var frames      = spriteSheet.width / spriteSheet.height;
            var dim         = spriteSheet.height;
            var scale       = dim / 32f; 
            _realHotspot    = _hotspot * scale;
            
            _sprites = new Texture2D[frames];
            for (var n = 0; n < frames; n++)
            {
                var tex = new Texture2D(dim, dim, TextureFormat.RGBA32, false, false, true);
                tex.SetPixels(spriteSheet.GetPixels(dim * n, 0, dim, dim));
                tex.Apply(false, false);
                _sprites[n] = tex;
            }
        }
    }
}