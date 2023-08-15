using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class TextBehaviour : PlayableBehaviour
    {
        public TextAsset.TextSource m_Source = TextAsset.TextSource.Content;
        
        public string   m_Text;
        public GvString m_Value;
    }
}