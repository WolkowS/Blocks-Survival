using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SetMainCameraBrain : MonoBehaviour
    {
        private void Awake()
        {
            var director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                var timelineAsset = (TimelineAsset)director.playableAsset;
                // iterate through tracks and map the objects appropriately
                foreach (var binding in timelineAsset.outputs.Where(n => n.outputTargetType == typeof(CinemachineBrain)))
                {
                    director.SetGenericBinding(binding.sourceObject, Core.Instance.CameraBrain);
                }
            }
        }
    }
}