using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/GlitchDigital")]
    public sealed class GlitchDigitalSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter _intensity = new ClampedFloatParameter(0f, 0f, 1f);

        // =======================================================================
        public bool IsActive() => active && _intensity.value > 0f;

        public bool IsTileCompatible() => true;
    }
}