using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Blur : FxTextureAsset.Pass
    {
        [CurveRange]
        public AnimationCurve _impact = AnimationCurve.Linear(0, 1, 1, 0);

        public float   _radius = 1;
        
        // =======================================================================
        public override void Apply(Color[] pixels, int width, int height)
        {
            var output = new Color[width * height];
            var bounds = new RectInt(0, 0, width, height);
            
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var weight = 0f;
                var color  = Color.clear;
                var center = new Vector2(x + .5f, y + .5f);

                foreach (var index in _sample(center, _radius))
                {
                    var pixel    = pixels[index.x + index.y * width];
                    var toPixel  = (index.ToVector2() + new Vector2(.5f, .5f)) - center;
                    var curvePos = toPixel.magnitude / _radius;
                    
                    if (curvePos > 1f)
                        continue;
                    
                    var impact = _impact.Evaluate(curvePos);
                    
                    weight += impact;
                    color += pixel * impact;
                }
                
                color /= weight; 
                
                // -----------------------------------------------------------------------
                IEnumerable<Vector2Int> _sample(Vector2 pos, float radius)
                {
                    var rect = new RectInt((pos.x - radius).RoundToInt(), (pos.y - radius).RoundToInt(), (radius * 2).RoundToInt(), (radius * 2).RoundToInt());
                    rect.ClampToBounds(bounds);
                    
                    foreach (var index in rect.Enumerate())
                    {
                        yield return index;
                    }
                }
                
                output[x + y * width] = color;
            }

            output.CopyTo(pixels, 0);
        }
    }
}