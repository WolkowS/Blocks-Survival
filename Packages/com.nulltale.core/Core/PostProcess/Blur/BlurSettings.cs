using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Blur")]
    public sealed class BlurSettings : VolumeComponent, IPostProcessComponent
    {

        public ClampedIntParameter   m_Directions    = new ClampedIntParameter(7, 1, 32);
        public ClampedFloatParameter m_Steps         = new ClampedFloatParameter(3, 1, 7);
        public ClampedFloatParameter m_Radius        = new ClampedFloatParameter(0, 0, 0.12f);
        public ClampedFloatParameter m_RotationSpeed = new ClampedFloatParameter(0, -21, 21);
        public ClampedFloatParameter m_Radial        = new ClampedFloatParameter(0, -7, 7);

        // =======================================================================

        // Can be used to skip rendering if false
        public bool IsActive() => active && m_Radius.value > 0;

        public bool IsTileCompatible() => false;
    }
}