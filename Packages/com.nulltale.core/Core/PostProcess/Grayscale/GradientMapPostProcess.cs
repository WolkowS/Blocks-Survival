using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class GradientMapPostProcess : PostProcess.Pass
    {
        private static readonly int s_Intensity   = Shader.PropertyToID("_Intensity");
        private static readonly int s_Gradient = Shader.PropertyToID("_Gradient");
        private static readonly int s_Weights = Shader.PropertyToID("_Weights");
        
        public Texture2D _gradient;

        public Optional<Vector4> _weights = new Optional<Vector4>(new Vector4(.299f, .587f, .114f, 1f), false);
        
        
        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<GradientMapSettings>();

            if (settings.IsActive() == false)
                return false;
            
            var tex = settings.m_Gradient.value;
            if (tex == null)
                tex = _gradient;

            mat.SetTexture(s_Gradient, tex);
            mat.SetFloat(s_Intensity, settings.m_Intensity.value);
            
            if (_weights.Enabled)
                mat.SetVector(s_Weights, _weights.Value);
            else
                mat.SetVector(s_Weights, new Vector4(.299f, .587f, .114f, 1f));
            
            return true;
        }
    }
}