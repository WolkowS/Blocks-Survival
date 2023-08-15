using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Pixelation")]
    public sealed class PixelationSettings : VolumeComponent, IPostProcessComponent
    {
        private const float k_NotPixelated = 1f;

        public ClampedFloatParameter m_Scale = new ClampedFloatParameter(k_NotPixelated, 0, 1f);

        // =======================================================================
        public bool IsActive() => active && m_Scale.value < k_NotPixelated;

        public bool IsTileCompatible() => false;
    }
}