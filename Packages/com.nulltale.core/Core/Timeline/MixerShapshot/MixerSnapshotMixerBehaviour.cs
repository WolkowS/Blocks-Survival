using CoreLib.Sound;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    public class MixerSnapshotMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // try to get mixer
            var audioMixer = SoundManager.Instance.Mixer;
            if (audioMixer == null)
                return;

            var inputCount = playable.GetInputCount();

            // calculate weights
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight = playable.GetInputWeight(n);

                var inputPlayable = (ScriptPlayable<MixerSnapshotBehaviour>)playable.GetInput(n);
                var input = inputPlayable.GetBehaviour();
                
                if (input.Snapshot == null)
                    return;

                // set blended weight of the snapshot
                input.m_SnapshotHandle.Weight = input.Weight * inputWeight;
            }
        }
    }
}