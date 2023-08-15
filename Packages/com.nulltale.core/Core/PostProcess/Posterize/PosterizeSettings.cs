using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Posterize")]
    public sealed class PosterizeSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedIntParameter m_Count = new ClampedIntParameter(64, 1, 64);

        // =======================================================================
        public bool IsActive() => active && m_Count.overrideState;

        public bool IsTileCompatible() => false;
    }
}