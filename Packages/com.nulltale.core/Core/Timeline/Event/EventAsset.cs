using System;
using CoreLib.Events;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace CoreLib.Timeline
{
    [NotKeyable]
    public class EventAsset : PlayableAsset, ITimelineClipAsset
    {
        public GlobalEvent m_Event;
        
        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<EventBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Event    = m_Event;
            
            return playable;
        }
    }
}