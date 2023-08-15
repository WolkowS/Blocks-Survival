using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/ShadowGrain")]
    public sealed class ShadowGrainSettings : VolumeComponent, IPostProcessComponent
    {
        public MinFloatParameter m_Impact      = new MinFloatParameter(0f, 0f);
        public ClampedFloatParameter m_Sensitivity = new ClampedFloatParameter(1f, 0f, 1f);
        public ColorParameter    m_Tint        = new ColorParameter(Color.white);
        

        // =======================================================================
        public bool IsActive() => m_Impact.value > 0f;

        public bool IsTileCompatible() => true;
    }
}