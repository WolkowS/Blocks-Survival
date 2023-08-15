using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    [Serializable, VolumeComponentMenu("Core/TvGlich")]
    public sealed class TvGlichSettings : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter  _enable  = new BoolParameter(false);
        public FloatParameter _hlScale = new FloatParameter(0f);
        public FloatParameter _hlSpeed = new FloatParameter(1f);
        public FloatParameter _tSpeed  = new FloatParameter(2f);
        public FloatParameter _tScale  = new FloatParameter(200f);
        public ColorParameter _color   = new ColorParameter(Color.clear);
        

        // =======================================================================
        public bool IsActive() => active && _enable.value;

        public bool IsTileCompatible() => true;
    }
}