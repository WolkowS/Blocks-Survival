using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Random : FxTextureAsset.PassCombine
    {
        public Gradient _gradient = new Gradient();
        public float _seed;
        public float _offset; 
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            return _fill(width, height);
        }

        public Color[] _fill(int width, int height)
        {
            var pixels = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                UnityEngine.Random.InitState((y + height * _offset + _seed * 10000f).RoundToInt());
                for (var x = 0; x < width; x++)
                {
                    var value = UnityEngine.Random.value;
                    pixels[x + y * width] = _gradient.Evaluate(value);
                }
            }

            return pixels;
        }
    }
}