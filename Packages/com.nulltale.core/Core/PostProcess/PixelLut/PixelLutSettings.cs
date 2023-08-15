using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/PixelLut")]
    public sealed class PixelLutSettings : VolumeComponent, IPostProcessComponent
    {
        private const float                 k_Contribution = 0f;
        public        Texture2DParameter    m_Lut          = new Texture2DParameter(null);
        public        ClampedFloatParameter m_Contribution = new ClampedFloatParameter(k_Contribution, 0, 1);

        // =======================================================================
        public bool IsActive() => active && m_Lut.value.IsNull() == false && m_Contribution.value > k_Contribution;

        public bool IsTileCompatible() => false;
    }
}