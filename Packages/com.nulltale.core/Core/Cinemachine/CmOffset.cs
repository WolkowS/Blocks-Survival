using Cinemachine;
using UnityEngine;

namespace CoreLib.Cinemachine
{
    [AddComponentMenu("")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CmOffset : CinemachineExtension
    {
        public static Vector3 s_Impact;
        
        public  CinemachineCore.Stage _applyAfter = CinemachineCore.Stage.Body;
        
        // =======================================================================
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (state.Lens.Orthographic == false)
                return;

            if (_applyAfter != stage)
                return;
            
            state.PositionCorrection += s_Impact;
        }
    }
}