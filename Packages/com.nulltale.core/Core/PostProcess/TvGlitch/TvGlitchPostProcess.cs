using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class TvGlitchPostProcess : PostProcess.Pass
    {
        private static readonly int  s_HlScaleId = Shader.PropertyToID("_HlScale");
        private static readonly int  s_HlSpeedId = Shader.PropertyToID("_HlSpeed");
        private static readonly int  s_TSpeedId  = Shader.PropertyToID("_TSpeed");
        private static readonly int  s_TScaleId  = Shader.PropertyToID("_TScale");
        private static readonly int  s_ColorId   = Shader.PropertyToID("_Color");

        protected override bool Invert => true;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<TvGlichSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_HlScaleId, settings._hlScale.value);
            mat.SetFloat(s_HlSpeedId, settings._hlSpeed.value);
            mat.SetFloat(s_TSpeedId, settings._tSpeed.value);
            mat.SetFloat(s_TScaleId, settings._tScale.value);
            mat.SetColor(s_ColorId, settings._color.value);
            
            return true;
        }
    }
}