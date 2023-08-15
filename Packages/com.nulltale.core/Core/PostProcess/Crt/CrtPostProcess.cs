using System;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CoreLib.Render
{
    public class CrtPostProcess : PostProcess.Pass
    {
        private static readonly int              s_NoiseTex    = Shader.PropertyToID("_NoiseTex");
        private static readonly int              s_NoiseOffset = Shader.PropertyToID("_NoiseOffset");
        private static readonly int              s_Distortion  = Shader.PropertyToID("_Distortion");
        private static readonly int              s_Scanlines   = Shader.PropertyToID("_Scanlines");
        
        
        public  float            _flipRelease = .1f;
        public  AnimationCurve01 _flipCurve;
        public  NoiseSettings    _noiseSettings;
        private float            _flip;
        private float            _flicker;
        private Texture2D        _noise;

        [Serializable]
        public class NoiseSettings
        {
            public int             _height     = 180;
            public FilterMode      _filter = FilterMode.Point;
        }
        
        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<CrtSettings>();

            if (settings.IsActive() == false)
                return false;
            
            var aspect = Screen.width / (float)Screen.height;
            var noiseRes = new Vector2Int((int)(_noiseSettings._height * aspect), _noiseSettings._height);
            if (_noise == null || _noise.width != noiseRes.x || _noise.height != noiseRes.y)
            {
                _noise            = new Texture2D(noiseRes.x, noiseRes.y, TextureFormat.RGBA32, false);
                
                _noise.filterMode = _noiseSettings._filter;
                _noise.wrapMode = TextureWrapMode.Repeat;
                for (var x = 0; x < _noise.width; x++)
                for (var y = 0; y < _noise.height; y++)
                    _noise.SetPixel(x, y, new Color(Random.value, Random.value, Random.value, Random.value));
                
                _noise.Apply();
            }
            
            mat.SetTexture(s_NoiseTex, _noise);
            mat.SetVector(s_NoiseOffset, new Vector4(Random.value, Random.value, Random.value, Random.value));
            
            var flipSpeed = settings.m_Flip.value;
            var hasFlip = flipSpeed > 0;
            
            if (hasFlip || _flip != 0)
                _flip += (hasFlip ? flipSpeed : (_flip > .5f ? 1f / _flipRelease : -1f / _flipRelease)) * Time.deltaTime;
            
            if (hasFlip == false && (_flip > 1f || _flip < 0f))
                _flip = 0f;
            
            _flip %= 1f;
            
            _flicker += Time.deltaTime;
            
            if (_flicker > settings.m_FlickerPeriod.value)
                _flicker -= settings.m_FlickerPeriod.value;
            
            var flicker = Mathf.Sin((_flicker / (settings.m_FlickerPeriod.value + 0.07f)) * Mathf.PI * 2f) * settings.m_FlickerPower.value;
            var noise   = settings.m_NoiseIntensity.value;
                        if (noise == 0)
                            noise = -1;
            
            mat.SetVector(s_Distortion, new Vector4(settings.m_DistortionPower.value, settings.m_DistortionPeriod.value, settings.m_DistortionDensity.value, _flipCurve.Evaluate(_flip)));
            mat.SetVector(s_Scanlines, new Vector4(settings.m_ScanlinesCount.value, settings.m_ScanlinesIntensity.value, flicker, noise));
            
            return true;
        }
    }
}