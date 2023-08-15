using System;
using System.Collections.Generic;
using CoreLib.Values;

namespace CoreLib
{
    internal interface IPlayableValueHandle
    {
        internal void Set(float weight, float value);
    }

    internal interface IPlayableValue
    {
        internal IPlayableValueHandle Lock();
        internal void UnLock(IPlayableValueHandle handle);
    }

    internal interface IPlayableValueHandleOptions
    {
        internal void Init(SetMode mode, Ref<float> blend, int priority);
    }

    public interface IPriorityHandle
    {
        int Priority { get; }
    }

    internal sealed class PriorityRelationalComparer : IComparer<IPriorityHandle>
    {
        internal static IComparer<IPriorityHandle> Instance { get; } = new PriorityRelationalComparer();

        public int Compare(IPriorityHandle x, IPriorityHandle y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
   
    internal interface IPlayableUpdate
    {
        internal void Requiest();
        internal void Update();
    }

    public abstract class PlayableValueHandle<T> : IPlayableValueHandle, IPlayableValueHandleOptions, IPriorityHandle
    {
        internal GlobalValuePlayable<T>                   m_Owner;
        internal IPlayableUpdate                          m_PlayableUpdate;
        public   SortedCollection<PlayableValueHandle<T>> m_ApplySequence;

        public int        m_Priority;
        public SetMode    m_Mode;
        public Ref<float> m_Blend;
        public float      m_Weight = float.PositiveInfinity;
        public float      m_Value  = float.PositiveInfinity;
        
        public int Priority => m_Priority;

        // =======================================================================
        public abstract void Apply(ref T value);

        void IPlayableValueHandle.Set(float weight, float value)
        {
            if (m_Weight == weight && m_Value == value)
                return;

            m_Weight = weight;
            m_Value = value;

            m_PlayableUpdate.Requiest();
        }

        void IPlayableValueHandleOptions.Init(SetMode mode, Ref<float> blend, int priority)
        {
            m_Mode     = mode;
            m_Priority = priority;
            m_Blend    = blend;

            m_ApplySequence.Add(this);
        }

        protected PlayableValueHandle(GlobalValuePlayable<T> owner, SortedCollection<PlayableValueHandle<T>> applySequence)
        {
            m_Owner          = owner;
            m_PlayableUpdate = owner;
            m_ApplySequence  = applySequence;
        }
    }

    [Serializable]
    public enum SetMode
    {
        Override,
        Floor,
        Ceil,
        Add
    }

    [Serializable]
    public enum Blending
    {
        FromInitial,
        FromZero
    }
}