using System;
using CoreLib.Events;
using CoreLib.States;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [NotKeyable] 
    public class SkipAsset : PlayableAsset, ITimelineClipAsset
    {
        public Mode m_Mode = Mode.Skip;
        [ShowIf(nameof(m_Mode), Mode.Rewind)]
        public Vers<float> m_RewindSpeed = new Vers<float>(7f);
        [NonSerialized]
        internal SkipTrack m_Track;
        [NonSerialized]
        internal TimelineClip m_Clip;
        
        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        [Serializable]
        public enum Mode
        {
            Skip,
            Rewind
        }

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            switch (m_Mode)
            {
                case Mode.Skip:
                {
                    var playable  = ScriptPlayable<SkipBehaviourSkip>.Create(graph);
                    var behaviour = playable.GetBehaviour();
                    behaviour.m_Clip     = m_Clip;
                    behaviour.m_Track    = m_Track;
                    behaviour.m_Director = go.GetComponent<PlayableDirector>();

                    return playable;
                }
                case Mode.Rewind:
                {
                    var playable  = ScriptPlayable<SkipBehaviourRewind>.Create(graph);
                    var behaviour = playable.GetBehaviour();
                    behaviour.m_Clip     = m_Clip;
                    behaviour.m_Track  = m_Track;
                    behaviour.m_Director = go.GetComponent<PlayableDirector>();
                    behaviour.m_RewindSpeed = m_RewindSpeed.Value;

                    return playable;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}