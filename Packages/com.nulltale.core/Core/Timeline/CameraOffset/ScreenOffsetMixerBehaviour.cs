using CoreLib.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ScreenOffsetMixerBehaviour : PlayableBehaviour
    {
        private Vector3 m_Offset;
        private float   m_Ortho;
        private float   m_Roll;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_Offset = Vector3.zero;
            m_Ortho  = 0f;
            m_Roll   = 0f;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            CmOffset.s_Impact -= m_Offset;
            CmOrtho.s_Impact  -= m_Ortho;
            
            m_Offset = Vector3.zero;
            m_Ortho  = 0f;
            m_Roll   = 0f;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            // calculate weights
            var offset = Vector3.zero;
            var ortho  = 0f;
            var roll   = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                var inputPlayable = (ScriptPlayable<ScreenOffsetBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                offset += behaviour.m_Offset * behaviour.m_Weight * inputWeight;
                ortho  += behaviour.m_Ortho * behaviour.m_Weight * inputWeight;
                roll   += behaviour.m_Roll * behaviour.m_Weight * inputWeight;
            }

            CmOffset.s_Impact += offset - m_Offset;
            m_Offset = offset;
            
            CmOrtho.s_Impact += ortho - m_Ortho;
            m_Ortho = ortho;
            
            CmRoll.s_Impact += roll - m_Roll;
            m_Roll = roll;
        }
    }
}