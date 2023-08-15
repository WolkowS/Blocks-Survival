using UnityEngine;

namespace CoreLib
{
    public abstract class TimelinePlayerBase : MonoBehaviour, ITimelinePlayer
    {
        private Switch m_Lock;

        public void Play()
        {
            if (m_Lock.Up())
                _play();
        }

        public void Stop()
        {
            if (m_Lock.Down())
                _stop();
        }

        public abstract void _play();
        public abstract void _stop();
    }
}