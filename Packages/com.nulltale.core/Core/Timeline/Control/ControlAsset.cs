using System;
using CoreLib.States;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [NotKeyable]
    public class ControlAsset : PlayableAsset, ITimelineClipAsset
    {
        public GlobalStateBase                m_State;
        public ControlBehaviour.Condition m_While = ControlBehaviour.Condition.Open;
        public ControlBehaviour.Mode      m_Mode  = ControlBehaviour.Mode.Hold;
        
        [HideIf(nameof(m_Mode), ControlBehaviour.Mode.Repeat)] [MinValue(0)]
        public float    m_ReleaseTime;
        
        [SerializeField]
        public ExposedReference<GameObject> m_Activate;
        [NonSerialized]
        public TimelineClip            m_Clip;
        
        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<ControlBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Clip        = m_Clip;
            behaviour.m_State       = m_State;
            behaviour.m_Director    = go.GetComponent<PlayableDirector>();
            behaviour.m_While       = m_While;
            behaviour.m_Mode        = m_Mode;
            behaviour.m_ReleaseTime = m_ReleaseTime;
            behaviour.m_Activate    = m_Activate.Resolve(graph.GetResolver());
            
            if (behaviour.m_Activate != null)
                behaviour.m_Activate.gameObject.SetActive(false);

            return playable;
        }
    }
}