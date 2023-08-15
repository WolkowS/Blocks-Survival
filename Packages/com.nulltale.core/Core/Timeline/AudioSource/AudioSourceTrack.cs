using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(1f, 0.8682783f, 0f)]
    [TrackClipType(typeof(AudioSourceAsset))]
    [TrackBindingType(typeof(AudioSource))]
    public class AudioSourceTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<AudioSourceMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}