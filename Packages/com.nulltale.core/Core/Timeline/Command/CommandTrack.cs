using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.469052f, 0.5936575f, 0.596f)]
    [TrackClipType(typeof(CommandAsset), false)]
    //[TrackBindingType(typeof(GameObject))]
    public class CommandTrack : TrackAsset 
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var clips = GetClips();
            foreach(var clip in clips)
            {
                var clipAsset = (CommandAsset)clip.asset;
                clipAsset.m_Clip = clip;
            }
 
            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }
}