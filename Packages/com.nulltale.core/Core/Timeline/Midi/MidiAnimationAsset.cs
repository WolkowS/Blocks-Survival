using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Midi
{
    [Serializable]
    public sealed class MidiAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        public MidiAnimationBehaviour template = new MidiAnimationBehaviour();

        public override double duration => template.DurationInSecond;

        public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.Extrapolation | ClipCaps.Looping | ClipCaps.SpeedMultiplier;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return ScriptPlayable<MidiAnimationBehaviour>.Create(graph, template);
        }
    }
}
