using System;
using UnityEngine;

namespace CoreLib
{
    public interface IInvokable
    {
        void Invoke();
    }
    
    public interface ISwitch
    {
        public bool IsOn { get; }
        
        bool Up();
        bool Down();
    }

    public abstract class SwitchBase : MonoBehaviour, ISwitch
    {
        protected int m_Value;

        public bool IsOn  => m_Value > 0;
        public bool Value => IsOn;
        public bool Up()   => ++ m_Value == 1;
        public bool Down() => -- m_Value == 0;
        
        protected abstract void _activated();
        protected abstract void _deactivated();
    }

    [Serializable]
    public struct Switch : IRefGet<bool>, ISwitch
    {
        [SerializeField]
        private int m_Value;

        public bool IsOn  => m_Value > 0;
        public bool Value => IsOn;
        public bool Up()   => ++ m_Value == 1;
        public bool Down() => -- m_Value == 0;

        /// <summary>
        /// returns true if lock state was changed
        /// </summary>
        public bool Apply(bool inc) => inc ? Up() : Down();

        public void Reset() => m_Value = 0;
        
        public static implicit operator bool(Switch l)
        {
            return l.m_Value > 0;
        }

    }
}