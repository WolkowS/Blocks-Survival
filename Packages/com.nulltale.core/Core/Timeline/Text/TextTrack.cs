using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.76f, 0.76f, 0.76f)]
    [TrackClipType(typeof(TextAsset))]
    [TrackBindingType(typeof(TMP_Text))]
    public class TextTrack : TrackAsset
    {
        [HideInInspector]
        public Methotd _method;

        // =======================================================================
        public enum Methotd
        {
            Alpha,
            Type,
        }
        
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<TextMixerBehaviour>.Create(graph, inputCount);
            var behaviour  = mixerTrack.GetBehaviour();
            behaviour._method = _method;
            
            return mixerTrack;
        }
    }
}