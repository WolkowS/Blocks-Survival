using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenOverlayAsset : PlayableAsset
    {
        [InplaceField(nameof(ScreenOverlayBehaviour.m_Scale), nameof(ScreenOverlayBehaviour.m_Color), nameof(ScreenOverlayBehaviour.m_Image), nameof(ScreenOverlayBehaviour.m_ScreenShot))]
        public ScreenOverlayBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<ScreenOverlayBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();

            return playable;
        }
    }
}