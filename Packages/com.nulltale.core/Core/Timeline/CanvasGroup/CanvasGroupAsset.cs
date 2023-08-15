using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class CanvasGroupAsset : PlayableAsset
    {
        [InplaceField(nameof(CanvasGroupBehaviour.m_Alpha), nameof(CanvasGroupBehaviour.m_BlockRaycasts), nameof(CanvasGroupBehaviour.m_Interactable))]
        public CanvasGroupBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<CanvasGroupBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();

            return playable;
        }
    }
}