using System;
using CoreLib.Scripting;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class SignalLink<T>
    {
        [SerializeField]
        private RefLink<Signal<T>> m_Ref;
        [SerializeField]
        private ResolverLink<Signal<T>> m_Resolver;
        [SerializeField]
        private Mode m_Mode;

        public Signal<T> Value => m_Mode switch
        {
            Mode.Ref      => m_Ref.Value,
            Mode.Resolver => m_Resolver.Value,
            _             => throw new ArgumentOutOfRangeException()
        };
        
        // =======================================================================
        public enum Mode
        {
            Ref,
            Resolver
        }
    }
}