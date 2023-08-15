using CoreLib.Render.RenderFeature;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace CoreLib.Render
{
    public class ScanlinesPostProcess : PostProcess.Pass
    {
        private static readonly int s_Scanlines = Shader.PropertyToID("_Scanlines");

        [CurveRange]
        public AnimationCurve  _intensity;
        public float           _intensitySpeed;
        [CurveRange]
        public AnimationCurve  _speedCurve;
        public float           _speedPeriod;
        [CurveRange]
        public AnimationCurve  _flicker;
        public  float _flickerSpeed;
        private float _offset;
        
        // =======================================================================
        public override void Init()
        {
            _offset = 0;
        }

        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ScanlinesSettings>();

            if (settings.IsActive() == false)
                return false;
            
            var intensity = _intensity.Evaluate(Time.time * _intensitySpeed) * settings.m_Intensity.value;
            var flicker   = _flicker.Evaluate(Time.time * _flickerSpeed) * settings.m_Flicker.value;
            _offset += _speedCurve.Evaluate(Time.time * _speedPeriod) * settings.m_Speed.value;
            
            while (_offset > 1f)
                _offset -= 1f;
            
            mat.SetVector(s_Scanlines, new Vector4(settings.m_Count.value, intensity, flicker, _offset));
            
            return true;
        }
    }
}