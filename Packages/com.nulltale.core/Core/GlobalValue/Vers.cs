using System;
using CoreLib.Scripting;
using UnityEngine;

namespace CoreLib.Values
{
    [Serializable]
    public sealed class Vers<T> : IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        private Mode             m_Mode;
        [SerializeField]     
        private T                m_Override;
        [SerializeField]     
        private GlobalValue      m_GlobalValue;
        [SerializeField]
        private RefLink<T>       m_Ref;
        [SerializeField]
        private ResolverLink<T>  m_Resolver;

        public bool IsRef => m_Mode != Mode.Override;
        
        public enum Mode
        {
            Override = 0,
            Global   = 1,
            Ref      = 2,
            Link     = 3,
        }
        
        public T Value
        {
            get
            {
                return m_Mode switch
                {
                    Mode.Override => m_Override,
                    Mode.Global   => ((IRefGet<T>)m_GlobalValue).Value,
                    Mode.Ref      => m_Ref.Value,
                    Mode.Link     => m_Resolver.Value,
                    _             => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch(m_Mode)
                {
                    case Mode.Override:
                        m_Override = value;
                        break;
                    case Mode.Global: 
                        ((IRefSet<T>)m_GlobalValue).Value = value;
                        break;
                    case Mode.Ref:
                        m_Ref.Value = value;
                        break;
                    case Mode.Link:
                        m_Resolver.Value = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public Vers()
        {
        }
        
        public Vers(T val)
        {
            m_Override = val;
        }
        
        public void Resolve()
        {
            switch(m_Mode)
            {
                case Mode.Ref:
                    m_Ref.Resolve();
                    break;
                case Mode.Link:
                    m_Resolver.Resolve();
                    break;
            }
        }
        
        public static implicit operator T(Vers<T> v) => v.Value;
    }
}