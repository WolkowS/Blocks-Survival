using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.8f, 0.2f, 0.2f)]
    [TrackClipType(typeof(StageAsset))]
    public class StageTrack : TrackAsset
    {
        public Optional<double> _start;
        public Optional<double> _end;
        public bool             _showUI = true;

        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var clips = GetClips();
            foreach(var clip in clips)
            {
                var clipAsset = (StageAsset)clip.asset;
                clipAsset._clip  = clip;
                clipAsset._track = this;
            }
 
            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }
}