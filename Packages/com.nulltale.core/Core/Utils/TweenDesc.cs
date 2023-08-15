using System;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TweenDesc
    {
        [CurveRange]
        public AnimationCurve _ease;
        public float          _time;
    }
}