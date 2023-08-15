using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Gradient Mapping")]
    public sealed class GradientMapSettings : VolumeComponent, IPostProcessComponent
    {
        private const float k_NoImpact = 0f;

        public ClampedFloatParameter m_Intensity = new ClampedFloatParameter(k_NoImpact, 0, 1f);
        public TextureParameter      m_Gradient  = new TextureParameter(null, TextureDimension.Tex2D);

        // =======================================================================
        public bool IsActive() => active && m_Intensity.value > k_NoImpact;

        public bool IsTileCompatible() => false;
    }
}