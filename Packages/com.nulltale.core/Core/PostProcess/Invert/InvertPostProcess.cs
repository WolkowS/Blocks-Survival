using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class InvertPostProcess : PostProcess.Pass
    {
        private static readonly int s_Weight = Shader.PropertyToID("_Weight");

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<InvertSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_Weight, settings.m_Weight.value);
            
            return true;
        }
    }
}