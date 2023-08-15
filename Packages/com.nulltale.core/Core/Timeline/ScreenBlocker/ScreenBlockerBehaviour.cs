using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ScreenBlockerBehaviour : PlayableBehaviour
    {
        private bool m_IsOpen;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying == false)
                return;

            if (m_IsOpen)
                return;

            m_IsOpen = true;
            UIBlocker.Open();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Application.isPlaying == false)
                return;
            
            if (m_IsOpen == false)
                return;

            m_IsOpen = false;
            UIBlocker.Close();
        }
    }
}