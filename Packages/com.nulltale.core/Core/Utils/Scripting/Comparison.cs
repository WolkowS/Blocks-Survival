using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CoreLib.Scripting
{
    public class Comparison<T> : MonoBehaviour
        where T : IComparable<T>
    {
        public Comparison m_Input;
        public Vers<T>    m_Value;

        public UnityEvent m_OnTrue;
        public UnityEvent m_OnFlase;

        // =======================================================================
        public void Invoke(MonoBehaviour iRef)
        {
            Invoke(((IRefGet<T>)iRef).Value);
        }

        public void Invoke(T val)
        {
            if (m_Input.Compare(val, m_Value.Value))
                m_OnTrue.Invoke();
            else
                m_OnFlase.Invoke();
        }
    }
    
    [Serializable]
    public enum Comparison
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Any
    }

    public static class ComparisonExtentions
    {
        public static bool Compare<T>(this Comparison comparison, T a, T b)
            where T : IComparable<T>
        {
            var compare = a.CompareTo(b);

            return comparison switch
            {
                Comparison.Equal          => compare == 0,
                Comparison.NotEqual       => compare != 0,
                Comparison.Less           => compare < 0,
                Comparison.LessOrEqual    => compare <= 0,
                Comparison.Greater        => compare > 0,
                Comparison.GreaterOrEqual => compare >= 0,
                Comparison.Any            => true,
                _                          => throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null)
            };
        }
    }
}