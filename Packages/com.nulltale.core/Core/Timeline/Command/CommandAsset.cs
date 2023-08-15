using System;
using CoreLib.Commands;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace CoreLib.Timeline
{
    [NotKeyable]
    public class CommandAsset : PlayableAsset, ITimelineClipAsset
    {
        public Cmd m_Asset;
        [SerializeField]
        public Optional<ExposedReference<Object>> m_Args = new Optional<ExposedReference<Object>>();
        [NonSerialized]
        public TimelineClip m_Clip;
        public bool m_Wait;
        
        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<CommandBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Clip     = m_Clip;
            behaviour.m_Cmd  = m_Asset;
            behaviour.m_Wait     = m_Wait;
            behaviour.m_Director = go.GetComponent<PlayableDirector>();
            behaviour.m_Args     = m_Args.Enabled ? new Optional<Object>(m_Args.Value.Resolve(graph.GetResolver()), true) : new Optional<Object>();
            
            return playable;
        }
    }
}