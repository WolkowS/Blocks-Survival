using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.8f, 0.2f, 0.3f)]
    [TrackClipType(typeof(ScreenOffsetAsset))]
    public class ScreenOffsetTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ScreenOffsetMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}