using System;
using UnityEngine;
using Cinemachine;

namespace CoreLib.Cinemachine
{
    [AddComponentMenu("")] // Hide in menu
    [ExecuteAlways]
    public class CmOrthoImpulseListener : CinemachineExtension
    {
        public CinemachineCore.Stage                      _applyAfter = CinemachineCore.Stage.Noise;
        public float                                      _gain = 1f;
        [CinemachineImpulseChannelProperty]
        public int                                        _channelMask;
        public CinemachineImpulseListener.ImpulseReaction _reactionSettings;

        public Mode _mode;
        
        // =======================================================================
        public enum Mode
        {
            Magnitude,
            Sum,
            Y
        }
        
        // =======================================================================
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == _applyAfter && deltaTime >= 0)
            {
                var haveImpulse  = CinemachineImpulseManager.Instance.GetImpulseAt(state.FinalPosition, true, _channelMask, out var impulsePos, out var impulseRot);
                var haveReaction = _reactionSettings.GetReaction(deltaTime, impulsePos, out var reactionPos, out var reactionRot);

                if (haveImpulse)
                {
                    impulseRot =  Quaternion.SlerpUnclamped(Quaternion.identity, impulseRot, _gain);
                    impulsePos *= _gain;
                }
                if (haveReaction)
                {
                    impulsePos += reactionPos;
                    impulseRot *= reactionRot;
                }
                
                if (haveImpulse || haveReaction)
                {
                    ref var lenseSettings = ref state.Lens;
                    var impact = _mode switch
                    {
                        Mode.Magnitude => impulsePos.magnitude,
                        Mode.Sum       => impulsePos.x + impulsePos.y + impulsePos.z,
                        Mode.Y         => impulsePos.y,
                        _              => throw new ArgumentOutOfRangeException()
                    };
                    lenseSettings.OrthographicSize += impact;
                }
            }
        }
    }
}