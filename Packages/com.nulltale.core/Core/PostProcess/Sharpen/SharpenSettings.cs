using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Sharpen")]
    public sealed class SharpenSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Impact = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter m_Weight = new ClampedFloatParameter(0, 0, 7);
        
        // =======================================================================
        
        // Can be used to skip rendering if false
        public bool IsActive() => active && m_Impact.value > 0;

        public bool IsTileCompatible() => false;
    }
}