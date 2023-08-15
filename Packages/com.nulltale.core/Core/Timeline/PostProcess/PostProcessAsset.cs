using System;
using CoreLib.Module;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class PostProcessAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.Blending;
        
        [InplaceField(nameof(PostProcessBehaviour.Weight))]
        public PostProcessBehaviour m_Template;
        
        public VolumeProfile m_Volume;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            if (m_Volume == null)
                return Playable.Null;

            var playable  = ScriptPlayable<PostProcessBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();
            behaviour.Handle = FxTools.PostProcess(m_Volume);

            return playable;
        }
    }
}