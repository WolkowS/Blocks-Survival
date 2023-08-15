using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class BlurPostProcess : PostProcess.Pass
    {
        private static readonly int s_Directions = Shader.PropertyToID("_Directions");
        private static readonly int s_Steps      = Shader.PropertyToID("_Steps");
        private static readonly int s_Radius     = Shader.PropertyToID("_Radius");
        private static readonly int s_Rotation   = Shader.PropertyToID("_Rotation");
        private static readonly int s_Radial     = Shader.PropertyToID("_Radial");

        private float _rotation;
        
        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();

            if (settings.IsActive() == false)
                return false;
            
            var dir = (float)settings.m_Directions.value;
            
            _rotation += settings.m_RotationSpeed.value * Time.deltaTime; 
            _rotation %= Mathf.PI * 2f;

            mat.SetFloat(s_Directions, dir);
            mat.SetFloat(s_Steps, settings.m_Steps.value);
            mat.SetFloat(s_Radius, settings.m_Radius.value);
            mat.SetFloat(s_Rotation, _rotation);
            mat.SetFloat(s_Radial, settings.m_Radial.value);
            
            return true;
        }
    }
}