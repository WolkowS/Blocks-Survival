using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Invert")]
    public sealed class InvertSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Weight = new ClampedFloatParameter(0, 0, 1);
        
        // =======================================================================
        
        // Can be used to skip rendering if false
        public bool IsActive() => active && m_Weight.value > 0;

        public bool IsTileCompatible() => false;
    }
}