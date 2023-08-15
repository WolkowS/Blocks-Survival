using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class CanvasGroupBehaviour : PlayableBehaviour
    {
        [Range(0, 1)]
        public float          m_Alpha = 1f;
        public Optional<bool> m_Interactable;
        public Optional<bool> m_BlockRaycasts;
    }
}