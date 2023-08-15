using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class PlayerBehaviour : PlayableBehaviour
    {
        public ITimelinePlayer m_TimelinePlayer;

        public ITimelinePlayer TimelinePlayer
        {
            get => m_TimelinePlayer;
            set
            {
                if (m_TimelinePlayer == value)
                    return;
                
                if (m_TimelinePlayer as Object != null)
                    m_TimelinePlayer.Stop();

                m_TimelinePlayer = value;

                if (m_TimelinePlayer as Object != null)
                    m_TimelinePlayer.Play();
            }
        }

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            TimelinePlayer = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            TimelinePlayer = (ITimelinePlayer)playerData;
        }
    }
}