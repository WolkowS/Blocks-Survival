using CoreLib.States;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class GlobalStateBehaviour : PlayableBehaviour
    {
        private GlobalStateBase m_GlobalState;

        // =======================================================================
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_GlobalState.IsNull() && playerData.IsNull() == false)
            {
                m_GlobalState = (GlobalStateBase)playerData;
                m_GlobalState.Open();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_GlobalState != null)
            {
                m_GlobalState.Close();
                m_GlobalState = null;
            }
        }
    }
}