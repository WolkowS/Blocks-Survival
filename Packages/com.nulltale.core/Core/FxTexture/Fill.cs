using System;
using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Fill : FxTextureAsset.PassCombine
    {
        public Gradient _gradient = new Gradient();
        public Type     _type;
        
        // =======================================================================
        [Serializable]
        public enum Type
        {
            Horizontal,
            Vertical
        }
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            switch (_type)
            {
                case Type.Horizontal:
                    return Horizontal(width, height);
                case Type.Vertical:
                    return Vertical(width, height);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public Color[] Horizontal(int width, int height)
        {
            var pixels = new Color[width * height];
                
            for (var x = 0; x < width; x++)
            {
                var color = _gradient.Evaluate((x + .5f) / width);
                for (var y = 0; y < height; y++)
                    pixels[x + y * width] = color;
            }
                
            return pixels;
        }
            
        public Color[] Vertical(int width, int height)
        {
            var pixels = new Color[width * height];
                
            for (var y = 0; y < height; y++)
            {
                var color = _gradient.Evaluate((y + .5f) / width);
                for (var x = 0; x < width; x++)
                    pixels[x + y * width] = color;
            }
                
            return pixels;
        }
    }
}