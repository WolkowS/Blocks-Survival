using System;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenShakeBehaviour : PlayableBehaviour
    {
        public float m_Amplitude = 1f;
        public float m_Torque;
        public float m_Frequency = 1f;
    }
}
