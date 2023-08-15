using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Chromatic")]
    public sealed class ChromaticSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter _Weight        = new ClampedFloatParameter(1f, 0f, 1f);
        public ClampedFloatParameter _Intensity     = new ClampedFloatParameter(0f, -.5f, .5f);
        public ClampedFloatParameter _RotationSpeed = new ClampedFloatParameter(0, -50, 50);
        
        // =======================================================================
        public bool IsActive() => active && _Intensity.value != 0f;

        public bool IsTileCompatible() => true;
    }
}