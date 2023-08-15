using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenOffsetBehaviour : PlayableBehaviour
    {
        public Vector3 m_Offset;
        public float   m_Ortho;
        public float   m_Roll;
        [Range(-1, 1)]
        public float   m_Weight = 1f;
    }
}
