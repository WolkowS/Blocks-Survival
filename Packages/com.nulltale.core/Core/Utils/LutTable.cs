using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate]
    public class LutTable : ScriptableObject
    {
        public Texture2D       _ramps;
        public bool            _indexed;
        
        public Optional<int>   _rampsDepth = new Optional<int>(7, false);
        public Optional<int>   _rampsCount = new Optional<int>(21, false);
        public  LutSize        _lutSize;
        public bool            _saveIndexedLut;
        
        [HideInInspector]
        public Texture2D      _lut;
        [HideInInspector]
        public Texture2D      _lutIndexed;
        [HideInInspector]
        public Texture2D      _lutGenerated;
        
        // =======================================================================
        public enum LutSize
        {
            x16,
            x32,
            x64
        }
        
        public class Grade
        {
            public Color[]      grade;
            public Vector2Int[] shape;

            // =======================================================================
            public Grade(Color[] grade, Vector2Int[] shape)
            {
                this.grade = grade;
                this.shape = shape;
            }
        }
        
        // =======================================================================
        public void GenerateIndex()
        {
            _lutIndexed = _getLut().Copy();
            
            var lut       = _lutIndexed.GetPixels();
            var colors    = new List<Color>();
            var gradsSize = new Vector2Int(_rampsCount.GetValueOrDefault(_ramps.width), _rampsDepth.GetValueOrDefault(_ramps.height));
            
            var gradient = _ramps.GetPixels(0, _ramps.height - gradsSize.y, gradsSize.x, gradsSize.y, 0).ToArray2D(gradsSize.x, gradsSize.y);
            for (var x = 0; x < gradient.GetLength(0); x++)
            {
                var grade = gradient.GetColumn(x).Reverse().ToArray();
                var color = grade.First();
                
                colors.Add(color);
            }

            // grade colors from lut to palette by rgb 
            var pixels = lut.Select(lutColor => colors.Select(gradeColor => (grade: compare(lutColor, gradeColor), color: gradeColor)).OrderBy(n => n.grade).First())
                            .Select(n => n.color)
                            .ToArray();
            
            _lutIndexed.SetPixels(pixels);
            _lutIndexed.Apply();
            
#if UNITY_EDITOR
            if (_saveIndexedLut)
            {
                var path = $"{Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this))}\\{name} Indexed.png";
                File.WriteAllBytes(path, _lutIndexed.EncodeToPNG());
                UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
                _setImportOptions(UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path), true);
            }
#endif

            // -----------------------------------------------------------------------
            float compare(Color a, Color b)
            {
                // compare colors by grayscale distance
                var weight = new Vector3(0.299f, 0.587f, 0.114f);
                var c = a.ToVector3().Mul(weight) - b.ToVector3().Mul(weight);
                
                return c.magnitude;
            }
        }
        
        // =======================================================================
        internal int _getLutSize()
        {
            return _lutSize switch
            {
                LutSize.x16 => 16,
                LutSize.x32 => 32,
                LutSize.x64 => 64,
                _           => throw new ArgumentOutOfRangeException()
            };
        }
        
        internal Texture2D _getLut()
        {
            var lutSize = _getLutSize();
            if (_lut != null && _lut.height == lutSize)
                return _lut;
            
            _lut            = new Texture2D(lutSize * lutSize, lutSize, TextureFormat.RGBA32, 0, false);
            _lut.filterMode = FilterMode.Point;

            for (var y = 0; y < lutSize; y++)
                for (var x = 0; x < lutSize * lutSize; x++)
                {
                    _lut.SetPixel(x, y, _lutAt(x, y));
                }
            
            _lut.Apply(false, false);
            
            return _lut;
        }
        
        private Color _lutAt(int x, int y)
        {
            var lutSize = _getLutSize();
            return new Color((x % lutSize) / (lutSize - 1f), y / (lutSize - 1f), (x / (float)lutSize).FloorToInt() * (1f / (lutSize - 1f)), 1f);
        }
        
        private (int x, int y, int z) _to3D(int x, int y)
        {
            var lutSize = _getLutSize();
            return (x % lutSize, y, (x / (float)lutSize).FloorToInt());
        }
        
        private (int x, int y) _to2D(int x, int y, int z)
        {
            var lutSize = _getLutSize();
            return (x + z * lutSize, y);
        }
        
        private Color _lutAt(int x, int y, int z)
        {
            var lutSize = _getLutSize();
            return new Color(x / (lutSize - 1f), y / (lutSize - 1f), z / (lutSize - 1f), 1f);
        }
        
#if UNITY_EDITOR
        public static void _setImportOptions(Texture2D tex, bool readable, bool import = true)
        {
            var path     = UnityEditor.AssetDatabase.GetAssetPath(tex);
            var importer = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
            importer.alphaSource         = UnityEditor.TextureImporterAlphaSource.FromInput;
            importer.anisoLevel          = 0;
            importer.textureType         = UnityEditor.TextureImporterType.Default;
            importer.textureCompression  = UnityEditor.TextureImporterCompression.Uncompressed;
            importer.filterMode          = FilterMode.Point;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture         = false;
            importer.isReadable          = readable;
            importer.mipmapEnabled       = false;
            importer.npotScale           = UnityEditor.TextureImporterNPOTScale.None;
            
            var texset = importer.GetDefaultPlatformTextureSettings();
            texset.format              = UnityEditor.TextureImporterFormat.RGBA32;
            texset.crunchedCompression = false;
            importer.SetPlatformTextureSettings(texset);
            
            if (import)
                UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
        }
        
        [Button]
        public void CreateRamps()
        {
            var gradsPath = $"{Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this))}\\{name}_Ramps.png";
            
            var gradsClean = new Texture2D(7, 7);
            for (var x = 0; x < gradsClean.width; x++)
            for (var y = 0; y < gradsClean.height; y++)
                gradsClean.SetPixel(x, y, Color.HSVToRGB(x / (float)gradsClean.width, 0f, (y + 3 - x) / (float)gradsClean.height));
            
            gradsClean.Apply();
            
            File.WriteAllBytes(gradsPath, gradsClean.EncodeToPNG());
            UnityEditor.AssetDatabase.ImportAsset(gradsPath, UnityEditor.ImportAssetOptions.ForceUpdate);

            _dealyed();
            
            // -----------------------------------------------------------------------
            async void _dealyed()
            {
                await Task.Yield();
                await Task.Yield();
                await Task.Yield();
                
                _setImportOptions(UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(gradsPath), true);
                
                await Task.Yield();
                await Task.Yield();
                await Task.Yield();
                
                _ramps = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(gradsPath);
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        
        [Button]
        public void Bake()
        {
            if (_ramps == null)
            {
                Debug.LogError($"{name} Can't bake lut, gradients is not set", this);
                return;
            }
                
            GenerateIndex();
            
            var lutSize   = _getLutSize();
            var width     = lutSize * lutSize;
            var height    = lutSize;
            var lut       = _lutIndexed.GetPixels().ToArray2D(width, height);
            var grades    = new List<Grade>();
            var gradsSize = new Vector2Int(_rampsCount.GetValueOrDefault(_ramps.width), _rampsDepth.GetValueOrDefault(_ramps.height - (_indexed ? 1 : 0)) + (_indexed ? 1 : 0));
            var tex       = new Texture2D(width, height * _rampsDepth.GetValueOrDefault(_ramps.height - (_indexed ? 1 : 0)), TextureFormat.RGBA32, false, false);
            
            // collect shapes
            var gradient = _ramps.GetPixels(0, _ramps.height - gradsSize.y, gradsSize.x, gradsSize.y, 0).ToArray2D(gradsSize.x, gradsSize.y);
            for (var x = 0; x < gradient.GetLength(0); x++)
            {
                var grade = gradient.GetColumn(x).Reverse().ToArray();
                var shape = _colorShape(grade.First()).ToArray();
                
                grades.Add(new Grade(grade, shape));
            }
            
            // draw gradient luts
            var pixels = new Color[tex.width, tex.height];
            for (var row = 0; row < gradsSize.y + (_indexed ? -1 : 0); row++)
            {
                var offset = new Vector2Int(0, (gradsSize.y - (row + 1 + (_indexed ? 1 : 0))) * lutSize);

                foreach (var grade in grades)
                {
                    var gradeColor = grade.grade[row + (_indexed ? 1 : 0)];
                    foreach (var pos in grade.shape)
                    {
                        var colorPos = pos + offset;
                        pixels[colorPos.x, colorPos.y] = gradeColor;
                    }
                }
            }
            
            tex.SetPixels(pixels.ToArray());
            tex.Apply();
            
            var path = $"{Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this))}\\{name}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());

            UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
            _lutGenerated = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path); 
            _setImportOptions(_lutGenerated, false);

            // -----------------------------------------------------------------------
            IEnumerable<Vector2Int> _colorShape(Color color)
            {
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    if (lut[x, y] == color)
                        yield return new Vector2Int(x, y);
                }
            }
        }
#endif
    }
}