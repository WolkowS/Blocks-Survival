using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/Outline")]
    public sealed class OutlineSettings : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Thickness = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter m_Sensitive = new ClampedFloatParameter(.02f, 0, 1);
        public ColorParameter        m_Color     = new ColorParameter(Color.clear);

        // =======================================================================
        public bool IsActive() => active;

        public bool IsTileCompatible() => false;
    }
}