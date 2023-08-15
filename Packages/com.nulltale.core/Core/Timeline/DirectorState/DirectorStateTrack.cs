using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    [ExcludeFromPreset]
    [TrackColor(0.37f, 0.37f, 0.77f)]
    [TrackClipType(typeof(ValueAsset))]
    [TrackBindingType(typeof(DirectorState))]
    public class DirectorStateTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ValueMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}