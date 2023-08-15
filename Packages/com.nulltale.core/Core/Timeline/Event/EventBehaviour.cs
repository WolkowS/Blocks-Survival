using System;
using CoreLib.Events;
using CoreLib.States;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace CoreLib.Timeline
{
    public class EventBehaviour : PlayableBehaviour
    {
        public  GlobalEvent          m_Event;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            (m_Event as GeVoid).Invoke();
        }
    }
}
