using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class TimelinePlayer : TimelinePlayerBase
    {
        [SerializeField]
        private UnityEvent m_Play;
        [SerializeField]
        private UnityEvent m_Stop;

        // =======================================================================
        public override void _play() => m_Play.Invoke();
        public override void _stop() => m_Stop.Invoke();
    }
}