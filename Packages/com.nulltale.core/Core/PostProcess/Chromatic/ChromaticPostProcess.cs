using System;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class ChromaticPostProcess : PostProcess.Pass
    {
        private static readonly int s_Weight    = Shader.PropertyToID("_Weight");
        private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int s_R         = Shader.PropertyToID("_R");
        private static readonly int s_G         = Shader.PropertyToID("_G");
        private static readonly int s_B         = Shader.PropertyToID("_B");

        public  Optional<Mode>  _mode;
        public  Optional<float> _angle;
        private float           _angleOffset;

        // =======================================================================
        public enum Mode
        {
            Offset,
            Rectangle
        }
        
        // =======================================================================
        public override void Init()
        {
            if (_mode.Enabled)
            {
                foreach (var keyword in _material.shaderKeywords)
                    _material.DisableKeyword(keyword);

                switch (_mode.Value)
                {
                    case Mode.Offset:
                    {
                        _material.EnableKeyword("OFFSET_MODE");
                    }
                        break;
                    case Mode.Rectangle:
                    {
                        _material.EnableKeyword("RECTANGLE_MODE");
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _material.SetVector(s_R, Vector2.left);
            _material.SetVector(s_G, Vector2.right);
            _material.SetVector(s_B, Vector2.up);
        }

        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ChromaticSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_Intensity, settings._Intensity.value);
            
            _angleOffset += settings._RotationSpeed.value * Time.deltaTime;
            _angleOffset %= (Mathf.PI * 2f);
            
            var step = (Mathf.PI * 2f) / 3f;
            _material.SetVector(s_R, (_angle + _angleOffset + step * 1f).ToNormal());
            _material.SetVector(s_G, (_angle + _angleOffset + step * 2f).ToNormal());
            _material.SetVector(s_B, (_angle + _angleOffset + step * 3f).ToNormal());
            
            _material.SetFloat(s_Weight, settings._Weight.value);
            
            return true;
        }
    }
}