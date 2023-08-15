using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class PosterizePostProcess : PostProcess.Pass
    {
        private static readonly int s_Count = Shader.PropertyToID("_Count");

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<PosterizeSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_Count, settings.m_Count.value);
            return true;
        }
    }
}