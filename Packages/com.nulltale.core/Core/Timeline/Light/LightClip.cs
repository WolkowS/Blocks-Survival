using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace CoreLib.Timeline
{
    [Serializable]
    public class LightClip : PlayableAsset, ITimelineClipAsset
    {
        public LightBehaviour template = new LightBehaviour();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightBehaviour>.Create(graph, template);
            return playable;
        }
    }
}