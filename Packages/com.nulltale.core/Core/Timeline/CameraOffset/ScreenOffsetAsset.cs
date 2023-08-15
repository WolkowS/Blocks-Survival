using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenOffsetAsset : PlayableAsset
    {
        [InplaceField(nameof(ScreenOffsetBehaviour.m_Offset), nameof(ScreenOffsetBehaviour.m_Ortho), nameof(ScreenOffsetBehaviour.m_Roll), nameof(ScreenOffsetBehaviour.m_Weight))]
        public ScreenOffsetBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<ScreenOffsetBehaviour>.Create(graph, m_Template);

            return playable;
        }
    }
}