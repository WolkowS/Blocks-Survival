using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.28f, 0.28f, 1f)]
    [TrackClipType(typeof(ValueAsset))]
    [TrackClipType(typeof(ProgressAsset))]
    [TrackBindingType(typeof(GlobalValue))]
    public class GlobalValueTrack : TrackAsset
    {
        public int        m_Priority;
        public SetMode    m_Mode;
        [HideIf(nameof(m_Mode), SetMode.Add)]
        public Ref<float> m_Base;
        
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var clips = GetClips();
            foreach(var clip in clips)
            {
                if ((clip.asset is IClipContainer cc) == false)
                    continue;
                
                cc.Clip = clip;
            }
            
            var mixerTrack = ScriptPlayable<ValueMixerBehaviour>.Create(graph, inputCount);
            var behaviour  = mixerTrack.GetBehaviour();
            behaviour.m_Mode     = m_Mode;
            behaviour.m_Blend    = m_Base;
            behaviour.m_Priority = m_Priority;

            return mixerTrack;
        }
    }
}