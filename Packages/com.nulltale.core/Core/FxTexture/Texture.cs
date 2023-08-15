using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Texture : FxTextureAsset.Pass
    {
        public Texture2D _texture;
        
        // =======================================================================
        public override void Apply(Color[] pixels, int width, int height)
        {
            if (_texture == null)
                return;
            
            var _tex  = _texture.Rescale(_texture.width, _texture.height, TextureFormat.RGBA32);
            var xStep = _texture.width / (float)width;
            var yStep = _texture.height / (float)width;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var source =pixels[x + y * width] ; 
                var color = _tex.GetPixel((x * xStep).ClampUp(_tex.width).RoundToInt(), (y * yStep).ClampUp(_tex.height).RoundToInt());
                pixels[x + y * width] = Color.Lerp(source, color.WithA(1f), color.a);
            }
        }
    }
}