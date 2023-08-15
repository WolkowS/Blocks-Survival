using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.9454092f, 0.9779412f, 0.3883002f)]
    [TrackClipType(typeof(LightClip))]
    [TrackBindingType(typeof(Light))]
    public class LightTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<LightMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var trackBinding = director.GetGenericBinding(this) as Light;
            if (trackBinding == null)
                return;
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Color");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Intensity");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_Range");
            driver.AddFromName<Light>(trackBinding.gameObject, "m_BounceIntensity");
#endif
            base.GatherProperties(director, driver);
        }
    }
}