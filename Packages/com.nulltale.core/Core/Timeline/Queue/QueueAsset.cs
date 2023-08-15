using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [NotKeyable]
    public class QueueAsset : PlayableAsset, ITimelineClipAsset
    {
        public IdAsset     m_Queue;
        [NonSerialized]
        public TimelineClip m_Clip;

        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<QueueBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_QueueId    = m_Queue;
            behaviour.m_Clip     = m_Clip;
            behaviour.m_Director = go.GetComponent<PlayableDirector>();

            return playable;
        }

    }
}