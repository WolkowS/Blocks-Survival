using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Grid : FxTextureAsset.PassCombine
    {
        public Color      _background = Color.black;
        public Color      _grid       = Color.white;
        public int        _width      = 1;
        public int _interval = 4;
        public Vector2Int _intervalAdd   = new Vector2Int(0, 0);
        public Vector2Int _offset     = new Vector2Int(0, 0);
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            return _fill(width, height);
        }

        public Color[] _fill(int width, int height)
        {
            var pixels = new Color[width * height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var xBase = x + _offset.x;
                if (xBase < 0)
                    xBase = Mathf.Abs(xBase - _width);
                
                var yBase = y + _offset.y;
                if (yBase < 0)
                    yBase = Mathf.Abs(yBase - _width);
                
                var hitX = (_interval + _intervalAdd.x) != 0 && xBase % (_interval + _intervalAdd.x) < _width;
                var hitY = (_interval + _intervalAdd.y) != 0 && yBase % (_interval + _intervalAdd.y) < _width;
                pixels[x + y * width] = hitX || hitY ? _grid : _background;
            }

            return pixels;
        }
    }
}