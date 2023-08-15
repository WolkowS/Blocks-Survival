using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class TransformAsset : PlayableAsset, ITimelineClipAsset
    {
        public TransformBehaviour          template = new TransformBehaviour();
        public ExposedReference<Transform> start;
        public ExposedReference<Transform> end;
        [NonSerialized]
        public TimelineClip                clip;

        public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.Extrapolation;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TransformBehaviour>.Create(graph, template);
            var beh      = playable.GetBehaviour();
            beh.clip          = clip;
            beh.startLocation = start.Resolve(graph.GetResolver());
            beh.endLocation   = end.Resolve(graph.GetResolver());
            
            return playable;
        }
    }
}