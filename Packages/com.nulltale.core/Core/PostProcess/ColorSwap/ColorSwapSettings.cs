using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/ColorSwap")]
    public sealed class ColorSwapSettings : VolumeComponent, IPostProcessComponent
    {
        public Texture2DParameter    m_LutTable = new Texture2DParameter(null);
        public ClampedFloatParameter m_Weight   = new ClampedFloatParameter(0, 0, 1);
        public MinFloatParameter     m_Eval     = new MinFloatParameter(0, 0);
        public BoolParameter         m_Sharp    = new BoolParameter(false);

        // =======================================================================

        // Can be used to skip rendering if false
        //public bool IsActive() => active && (m_Flip.value != 0 || m_FlickerPower.value != 0 || m_DistortionPower.value != 0 || m_NoiseIntensity.value != 0 || m_ScanlinesIntensity.value != 0);
        public bool IsActive() => active && (m_Weight.value > 0f);

        public bool IsTileCompatible() => false;
    }
}