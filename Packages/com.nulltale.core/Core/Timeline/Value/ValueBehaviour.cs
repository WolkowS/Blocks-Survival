using System;
using CoreLib.Values;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ValueBehaviour : PlayableBehaviour
    {
        public float                 m_Value = 1f;
    }
}