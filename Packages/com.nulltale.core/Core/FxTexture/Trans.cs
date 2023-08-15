using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Trans : FxTextureAsset.Pass
    {
        public Vector3 _scale = Vector3.one;
        public Vector2 _offset;
        public float   _rotation;
        
        // =======================================================================
        public override void Apply(Color[] pixels, int width, int height)
        {
            var output = new Color[width * height]; 
            
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var pos = _transform(new Vector2(x, y), new Vector2(width, height), _offset, _scale, _rotation) - new Vector2(.5f, .5f);
                
                var index = new Vector2Int(pos.x.RoundToInt(), pos.y.RoundToInt());
                
                var color = Color.clear;
                if (index.x >= 0 && index.x < width && index.y >= 0 && index.y < height)
                    color = pixels[index.x + index.y * width];
                
                output[x + y * width] = color;
            }

            output.CopyTo(pixels, 0);
        }
    }
}