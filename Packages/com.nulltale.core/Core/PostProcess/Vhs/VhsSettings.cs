using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Vhs")]
    public sealed class VhsSettings : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter         _enable  = new BoolParameter(false);
        public ClampedFloatParameter _bleed   = new ClampedFloatParameter(1f, 0f, 100f);
        public ClampedFloatParameter _rocking = new ClampedFloatParameter(0.01f, 0f, 0.1f);
        
        // =======================================================================
        public bool IsActive() => active && _enable.value;

        public bool IsTileCompatible() => true;
    }
}