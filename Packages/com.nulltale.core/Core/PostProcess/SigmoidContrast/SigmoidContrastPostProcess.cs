using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    public class SigmoidContrastPostProcess : PostProcess.Pass
    {
        private static readonly int s_Weight = Shader.PropertyToID("_Weight");
        private static readonly int s_Gamma  = Shader.PropertyToID("_Gamma");
        
        protected override bool Invert => true;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<SigmoidContrastSettings>();

            if (settings.IsActive() == false)
                return false;
            
            mat.SetFloat(s_Weight, settings._weight.value);
            mat.SetFloat(s_Gamma, settings._gamma.value);

            return true;
        }
    }
}