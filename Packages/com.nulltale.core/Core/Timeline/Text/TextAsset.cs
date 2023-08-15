using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class TextAsset : PlayableAsset//, ITimelineClipAsset
    {
        public TextSource m_Source = TextSource.Content;
        
        [ResizableTextArea]
        [ShowIf(nameof(m_Source), TextSource.Content)]
        public string m_Text;
        [ShowIf(nameof(m_Source), TextSource.GlobalValue)]
        public GvString m_Value;

        // =======================================================================
        public enum TextSource
        {
            Content,
            GlobalValue,
            Initial,
        }
        
        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<TextBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Source = m_Source;
            behaviour.m_Text   = m_Text;
            behaviour.m_Value  = m_Value;

            return playable;
        }

        //public ClipCaps clipCaps => 
    }
}