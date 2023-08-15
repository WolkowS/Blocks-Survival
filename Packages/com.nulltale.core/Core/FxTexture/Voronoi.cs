using ProceduralNoiseProject;
using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Voronoi : FxTextureAsset.PassCombine
    {
        public Gradient _gradient = new Gradient();
        public Vector3  _scale    = Vector3.one;
        public Vector2  _offset;
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            return _fill(width, height);
        }

        public Color[] _fill(int width, int height)
        {
            var pixels = new Color[width * height];
            var noise  = new VoronoiNoise(0, _scale.z);
            
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var value = noise.Sample2D(((x + .5f) / width + _offset.x) * _scale.x, ((y + .5f) / height + _offset.y) * _scale.y);
                pixels[x + y * width] = _gradient.Evaluate(value);
            }

            return pixels;
        }
    }
}