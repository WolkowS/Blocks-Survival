using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ScreenShakeMixerBehaviour : PlayableBehaviour
    {
        public Module.FxTools.NoiseHandle m_Noise;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_Noise = Module.FxTools.Noise();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_Noise?.Dispose();
            m_Noise = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_Noise == null)
                return;

            var inputCount = playable.GetInputCount();

            // calculate weights
            var amplitude = 0f;
            var frequency = 0f;
            var torque = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                var inputPlayable = (ScriptPlayable<ScreenShakeBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                amplitude += behaviour.m_Amplitude * inputWeight;
                torque    += behaviour.m_Torque * inputWeight;
                frequency += behaviour.m_Frequency * inputWeight;
            }

            m_Noise.Amplitude = amplitude;
            m_Noise.Frequency = frequency;
            m_Noise.Torque = torque;
        }
    }
}