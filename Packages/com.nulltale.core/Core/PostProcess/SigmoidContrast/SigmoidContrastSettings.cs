using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Contrast")]
    public sealed class SigmoidContrastSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter         _weight = new ClampedFloatParameter(0f, 0f, 1f);
        public NoInterpClampedFloatParameter _gamma  = new NoInterpClampedFloatParameter(.55f, 0f, 1f);

        // =======================================================================
        public bool IsActive() => active && _weight.value > 0f;

        public bool IsTileCompatible() => true;
    }
}