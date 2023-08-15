using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class AudioSourceAsset : PlayableAsset
    {
        [Range(0, 1)]
        public float m_Volume = 1f;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<AudioSourceBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Volume = m_Volume;

            return playable;
        }
    }
}