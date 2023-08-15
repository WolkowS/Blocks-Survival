using System;
using System.Collections.Generic;
using CoreLib.Module;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib.Values
{
    public abstract class Value : ScriptableObject
    {
    }

    public class GlobalValueProgress<T> : GvInt
    {
        [NonSerialized] [Label("Level Value")]
        public Ref<T>  Level = new Ref<T>(default);
        public List<T> _level;

        public override int Value
        {
            get => m_Value;
            set
            {
                if (Equals(m_Value, value))
                    return;

                m_Value     = value;
                Level.Value = _level[value];
                
                _invokeOnChanged(m_Value);
            }
        }

        public override void Init()
        {
            // initialize hack to call callback
            m_Value = m_InitialValue + 1;
            base.Init();
        }
    }

    [SoCreate]
    public abstract class GlobalValue : Value
    {
        public virtual void Init() {}

        internal virtual string Serialize() => string.Empty;
        internal virtual void Deserialize(string data) {}
    }

    public interface IContainerValue<T>
    {
        T Value { get; set; }
    }
    
    public interface IGlobalValue<T> : IContainerValue<T>
    {
        event Action<T>  OnChanged;
    }
    public abstract class GlobalValueReadonly<T> : GlobalValue, IContainerValue<T>, IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        protected T m_Value;
        
        public virtual T Value { get => m_Value; set => throw new ArgumentException(); }
    }

    public abstract class GlobalValue<T> : GlobalValueReadonly<T>, IGlobalValue<T>, IRefGet<T>, IRefSet<T>
    {
        public event Action<T>  OnChanged;
        
        public override T Value
        {
            get => m_Value;
            set
            {
                if (Equals(m_Value, value))
                    return;

                m_Value = value;
                OnChanged?.Invoke(m_Value);
            }
        }

        [SerializeField]
        protected T m_InitialValue;
        
        [SerializeField] [ResizableTextArea]
        private string m_Note;

        // =======================================================================
        public override void Init()
        {
            OnChanged = null;
            Value     = m_InitialValue;
        }
        
        private void OnValidate()
        {
            _invokeOnChanged(m_Value);
        }

        protected void _invokeOnChanged(T value) => OnChanged?.Invoke(value);
        
        public static implicit operator T(GlobalValue<T> val) => val.Value;
    }

    public abstract class GlobalValuePlayable<T> : GlobalValue<T>, IGlobalValue<T>, IRefGet<T>, IRefSet<T>, IPlayableValue, IPlayableUpdate
    {
        public override T Value
        {
            get => m_Value;
            set
            {
                if (IsLocked)
                {
                    m_CleanValue = value;
                    _setValue(GetPlayableValue());
                }
                else
                    _setValue(value);
            }
        }

        private  T    m_CleanValue;
        internal T    CleanValue => m_CleanValue;

        private  int  m_Lock;
        private  bool IsLocked => m_Lock > 0;
        
        private bool m_IsDirty;

        // =======================================================================
        public override void Init()
        {
            base.Init();
            m_IsDirty = false;
        }

        IPlayableValueHandle IPlayableValue.Lock()
        {
            if (m_Lock ++ == 0)
                m_CleanValue = m_Value;

            return OpenHandle();
        }

        void IPlayableValue.UnLock(IPlayableValueHandle handle)
        {
            if (-- m_Lock == 0)
                Value = m_CleanValue;

            CloseHandle(handle);
        }

        void IPlayableUpdate.Requiest()
        {
            if (Application.isPlaying == false)
            {
                ((IPlayableUpdate)this).Update();
                return;
            }

            if (m_IsDirty)
                return;

            m_IsDirty = true;
            
            GlobalValues.Instance.m_PlayableUpdate.m_Updates.Add(this);
        }

        void IPlayableUpdate.Update()
        {
            m_IsDirty = false;

            _setValue(GetPlayableValue());
        }

        internal abstract IPlayableValueHandle OpenHandle();
        internal abstract void CloseHandle(IPlayableValueHandle handle);
        internal abstract T GetPlayableValue();

        // =======================================================================
        protected void _setValue(T value)
        {
            if (Equals(m_Value, value))
                return;

            m_Value = value;
            _invokeOnChanged(m_Value);
        }


        public static implicit operator T(GlobalValuePlayable<T> val) => val.Value;
    }
}