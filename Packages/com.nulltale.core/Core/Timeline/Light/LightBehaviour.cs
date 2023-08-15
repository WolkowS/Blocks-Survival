using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class LightBehaviour : PlayableBehaviour
    {
        public Color color           = Color.white;
        public float intensity       = 1f;
        public float bounceIntensity = 1f;
        public float range           = 10f;
    }
}