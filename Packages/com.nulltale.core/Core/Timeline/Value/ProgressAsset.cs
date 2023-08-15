using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class ProgressAsset : PlayableAsset, IClipContainer, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.All;
        public TimelineClip Clip { get; set; }
        
        [InplaceField(nameof(ProgressBehaviour.m_Range), nameof(ProgressBehaviour.m_Lerp))]
        public ProgressBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<ProgressBehaviour>.Create(graph, m_Template);
            var beh      = playable.GetBehaviour();
            beh.m_Clip     = Clip;
            beh.m_Director = go.GetComponent<PlayableDirector>();
            
            return playable;
        }

    }
}