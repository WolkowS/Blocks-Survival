using System;
using System.Collections.Generic;
using System.Linq;
using SoCreator;
using UnityEngine;

namespace CoreLib.FxTexture
{
    [CreateAssetMenu(fileName = "FxTexture", menuName = "2D/FxTexture")]
    [SoCreate(true)]
    public class FxTextureAsset : ScriptableObject
    {
        public Vector2Int _size     = new Vector2Int(64, 64);
        public Gradient   _gradient = new Gradient();
        public bool       _point;
        public Texture2D  _texture;
        public List<Pass> _passes = new List<Pass>();

        // =======================================================================
        [SoCreate(false)]
        public abstract class Pass : ScriptableObject
        {
            public bool    _active = true;
            
            // =======================================================================
            public abstract void Apply(Color[] pixels, int width, int height);
            
            protected static Vector2 _transform(Vector2 pos, Vector2 size,  Vector2? offset, Vector3? scale, float? rotation = null)
            {
                var width  = size.x;
                var height = size.y;
                
                var aspect = (width * .5f) / (height * .5f);
                
                var xPos  = pos.x - width * .5f;
                var yPos  = pos.y - height * .5f;
                
                if (offset.HasValue)
                {
                    xPos += offset.Value.x * width;
                    yPos += offset.Value.y * height;
                }
                
                if (rotation != null)
                {
                    var right = rotation.Value.Deg2Rad().ToNormal();
                    var up    = right.Rotate90();
                    
                    var xBase = xPos + .5f;
                    var yBase = yPos + .5f;
                    xPos = xBase * right.x + yBase * up.x;
                    yPos = xBase * right.y + yBase * up.y;
                    xPos -= .5f;
                    yPos -= .5f;
                }
                
                if (scale.HasValue)
                {
                    xPos = xPos / scale.Value.x / scale.Value.z;
                    yPos = yPos / scale.Value.y / scale.Value.z;
                }

                xPos += width * .5f;
                yPos += height * .5f;
                
                return new Vector2(xPos + .5f, yPos * aspect + .5f);
            }
        }
        
        public abstract class PassCombine : Pass
        {
            public Combine _combine;
            
            // =======================================================================
            public override void Apply(Color[] pixels, int width, int height)
            {
                var source = _proceed(pixels, width, height);

                switch (_combine)
                {
                    case Combine.Add:
                    {
                        for (var x = 0; x < width; x++)
                        for (var y = 0; y < height; y++)
                            pixels[x + y * width] += source[x + y * width];
                    } break;
                    case Combine.Sub:
                    {
                        for (var x = 0; x < width; x++)
                        for (var y = 0; y < height; y++)
                        {
                            var c = pixels[x + y * width] - source[x + y * width];
                            pixels[x + y * width] = c.Clamp01();
                        }
                    } break;
                    case Combine.Mul:
                    {
                        for (var x = 0; x < width; x++)
                        for (var y = 0; y < height; y++)
                            pixels[x + y * width] *= source[x + y * width];
                    } break;
                    case Combine.Blend:
                    {
                        for (var x = 0; x < width; x++)
                        for (var y = 0; y < height; y++)
                        {
                            var initial = pixels[x + y * width];
                            var apply = source[x + y * width]; 
                            pixels[x + y * width] = Color.Lerp(initial, apply, apply.a).WithA(initial.a);
                        }
                    } break;
                    case Combine.Sample:
                    {
                        for (var x = 0; x < width; x++)
                        for (var y = 0; y < height; y++)
                            pixels[x + y * width] = source[x + y * width];
                    } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            public abstract Color[] _proceed(Color[] input, int width, int height);
        }

        [Serializable]
        public enum Combine
        {
            Mul,
            Add,
            Sub,
            Blend,
            Sample
        }
        
        // =======================================================================
        public void Fill()
        {
            Fill(_texture);
        }
        
        public void Fill(Texture2D texture)
        {
            var pixels = _getBase();

            foreach (var step in _passes.Where(n => n._active))
                step.Apply(pixels, _size.x, _size.y);
            
            texture.SetPixels(pixels);
            texture.Apply();
        }
        
        public Texture2D CreateTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = _point ? FilterMode.Point : FilterMode.Bilinear;
            texture.wrapMode   = TextureWrapMode.Clamp;
            
            Fill(texture);
            
            return texture;
        }
        
        public Texture2D CreateTexture()
        {
            return CreateTexture(_size.x, _size.y);
        }

        public Color[] GetPixels()
        {
            var pixels = _getBase();

            foreach (var step in _passes.Where(n => n._active))
                step.Apply(pixels, _size.x, _size.y);
            
            return pixels;
        }
        
        internal Color[] _getBase()
        {
            var width  = _size.x;
            var height = _size.y;
            
            var pixels = new Color[width * height];
            
            for (var x = 0; x < width; x++)
            {
                var color = _gradient.Evaluate((x + .5f) / width);
                for (var y = 0; y < height; y++)
                    pixels[x + y * width] = color;
            }
            
            return pixels;
        }
        
        // =======================================================================
        internal bool _validateSize(Vector2Int size)
        {
            return size.x.InRange(0, 2048) && size.y.InRange(0, 2048);
        }
        
        internal void _bake()
        {
            if (_texture == null)
            {
                _texture = CreateTexture();
            }
            else
            {
                _texture.Reinitialize(_size.x, _size.y, TextureFormat.RGBA32, false);
            }

            _texture.filterMode = _point ? FilterMode.Point : FilterMode.Bilinear;
            _texture.name       = name;
            
            Fill(_texture);
        }
    }
}