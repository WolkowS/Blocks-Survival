using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Cinemachine
{
    [AddComponentMenu("")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CmNoise : CinemachineExtension
    {
        private float m_Time;
        public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Body;
        [MinValue(0)]
        public float m_PositionImpact = 1;
        [MinValue(0)]
        public float m_RotationImpact = 1;

        // =======================================================================
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Noise)
                return;

            if (Module.FxTools.s_NoiseHandles.IsEmpty())
                return;

            if (Application.isPlaying == false)
            {
                deltaTime = Time.deltaTime;
            }
            else
            {
                if (deltaTime <= 0)
                    return;
            }

            var noise = Module.FxTools.s_NoiseHandles.MaxByOrDefault(n => n.Amplitude);
            m_Time += deltaTime * noise.Frequency;

            Module.FxTools.Instance.m_ShakeNoise.GetSignal(m_Time, out var pos, out var rot);

            if (noise.Amplitude > 0 && m_PositionImpact > 0)
            {
                var posImpact = pos * noise.Amplitude * m_PositionImpact;
                state.PositionCorrection += posImpact;
            }
            if (noise.Torque > 0 && m_RotationImpact > 0)
            { 
                var rotImpact = Quaternion.SlerpUnclamped(Quaternion.identity, rot, m_RotationImpact * noise.Torque);
                state.OrientationCorrection = state.CorrectedOrientation * rotImpact;
            }
        }
    }
}