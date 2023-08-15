using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class PixelationPostProcess : PostProcess.Pass
    {
        private static readonly int s_Pixels = Shader.PropertyToID("_Pixels");

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<PixelationSettings>();

            if (settings.IsActive() == false)
                return false;

            var val    = settings.m_Scale.value * Screen.height;
            var epsilon = 1f / (float)Screen.height;
            if (val < epsilon)
                val = epsilon;
            
            var aspect = Screen.width / (float)Screen.height;
            var pixels = new Vector4(val * aspect, val);
            
            mat.SetVector(s_Pixels, pixels);
            return true;
        }
    }
}