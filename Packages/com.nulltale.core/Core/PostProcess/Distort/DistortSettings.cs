using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Distort")]
    public sealed class DistortSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Weight    = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter m_Sharpness = new ClampedFloatParameter(0, 0, 3);
        public ClampedFloatParameter m_Tiling    = new ClampedFloatParameter(0, 0, 300);
        public ClampedFloatParameter m_Speed     = new ClampedFloatParameter(0, 0, 3);
        
        // =======================================================================
        public bool IsActive() => active && m_Weight.value > 0;

        public bool IsTileCompatible() => false;
    }
}