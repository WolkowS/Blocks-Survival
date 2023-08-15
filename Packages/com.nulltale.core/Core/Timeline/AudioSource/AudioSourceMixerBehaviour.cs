using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class AudioSourceMixerBehaviour : PlayableBehaviour
    {
        private Module.FxTools.ScreenOverlayHandle m_OverlayHandle;

        // =======================================================================
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var source = playerData as AudioSource;
            if (source == null)
                return;

            var inputCount = playable.GetInputCount();
            var volume = 0f;

            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight = playable.GetInputWeight(n);
                if (inputWeight <= 0f)
                    continue;

                var inputPlayable = (ScriptPlayable<AudioSourceBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                volume += behaviour.m_Volume * inputWeight;
            }

            source.volume = volume;
        }
    }
}