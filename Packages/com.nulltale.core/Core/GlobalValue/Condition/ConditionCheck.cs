using System;
using UnityEngine;

namespace CoreLib.Values.Condition
{
    [Serializable]
    public abstract class ConditionCheck : ScriptableObject
    {
        public abstract bool IsMet { get; }
    }
}