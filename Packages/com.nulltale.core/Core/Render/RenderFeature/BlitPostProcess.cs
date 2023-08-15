using UnityEngine;

namespace CoreLib.Render.RenderFeature
{
    public class BlitPostProcess : PostProcess.Pass
    {
        public bool _invert;

        protected override bool Invert => _invert;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            return true;
        }
    }
}