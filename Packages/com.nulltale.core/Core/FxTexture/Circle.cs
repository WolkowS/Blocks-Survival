using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Circle : FxTextureAsset.PassCombine
    {
        public Gradient _gradient = new Gradient();
        public Vector3  _scale    = Vector3.one;
        public Vector2  _offset;
        public float    _rotation;
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            return Center(width, height);
        }

        public Color[] Center(int width, int height)
        {
            var center = new Vector2(width * .5f, height * .5f);
            var pixels = new Color[width * height];
            var dist   = width * .5f + .1f;
            
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var pos                = _transform(new Vector2(x, y), new Vector2(width, height), _offset, _scale, _rotation);
                var impact             = ((pos - center).magnitude / dist).Abs();
                var color              = _gradient.Evaluate(impact);
                if (impact > 1f) color = Color.clear;
                pixels[x + y * width]  = color;
            }

            return pixels;
        }
    }

}