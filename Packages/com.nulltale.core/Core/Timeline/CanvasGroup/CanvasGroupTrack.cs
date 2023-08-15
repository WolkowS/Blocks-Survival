using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.76f, 0.76f, 0.76f)]
    [TrackClipType(typeof(CanvasGroupAsset))]
    [TrackBindingType(typeof(CanvasGroup))]
    public class CanvasGroupTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<CanvasGroupMixerBehaviour>.Create(graph, inputCount);
            var behaviour  = mixerTrack.GetBehaviour();
            
            return mixerTrack;
        }
    }
}