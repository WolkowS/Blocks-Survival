using Cinemachine;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Cinemachine
{
    [AddComponentMenu("")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CmOrtho : CinemachineExtension
    {
        public  static float s_Impact;
        
        public  Optional<GvFloat>     _impact;
        public  float                 _impactMul  = 1f;
        public  CinemachineCore.Stage _applyAfter = CinemachineCore.Stage.Body;
        public  float                 _smooth     = 10f;
        private float                 _apply;
        
        // =======================================================================
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (state.Lens.Orthographic == false)
                return;

            if (_applyAfter != stage)
                return;
            
            var impact = s_Impact;
            if (_impact.Enabled)
                impact += _impact.Value.Value;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                deltaTime = Time.deltaTime;
#endif
            
            _apply = Mathf.SmoothStep(_apply, impact * _impactMul, _smooth * deltaTime);
         
            ref var lenseSettings = ref state.Lens;
            lenseSettings.OrthographicSize += _apply;
        }
    }
}