using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.09803922f, 0.09803922f, 0.09803922f)]
    [TrackClipType(typeof(ScreenOverlayAsset))]
    public class ScreenOverlayTrack : TrackAsset
    {
        public Optional<int> m_SortingOrder = new Optional<int>(10000, true);
        
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ScreenOverlayMixerBehaviour>.Create(graph, inputCount);
            
            var behaviour  = mixerTrack.GetBehaviour();
            if (m_SortingOrder.Enabled)
                behaviour.m_SortingOrder = m_SortingOrder.Value;
            
            return mixerTrack;
        }
    }
}