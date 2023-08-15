using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Square : FxTextureAsset.PassCombine
    {
        public Gradient _gradient = new Gradient();
        public Vector3  _scale    = Vector3.one;
        public Vector2  _offset;
        public float    _rotation;
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            var pixels = new Color[width * height];
            
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var pos    = _transform(new Vector2(x, y), new Vector2(width, height), _offset, _scale, _rotation);
                var impact = Mathf.Max(((pos.x / (float)width - .5f) * 2f).Abs(), ((pos.y / (float)height - .5f) * 2f).Abs());
                var color  = _gradient.Evaluate(impact);
                
                if (((pos.x >= 0 && pos.x < width) && (pos.y >= 0 && pos.y < height)) == false)
                     color = Color.clear;
                
                if (impact > 1f)
                    color = Color.clear;
                
                pixels[x + y * width]  = color;
            }

            return pixels;
        }
    }
}