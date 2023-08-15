using System;
using CoreLib.Module;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class PostProcessBehaviour : PlayableBehaviour
    {
        [Range(0, 1)]
        public float Weight = 1;

        public FxTools.PostProcessInstance.Handle Handle;

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Handle.Weight = 0;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Handle.Weight = Weight * info.weight;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Handle.Dispose();
            Handle = null;
        }
    }
}