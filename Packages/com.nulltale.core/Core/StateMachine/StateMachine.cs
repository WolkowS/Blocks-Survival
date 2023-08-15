using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CoreLib
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }
    
    public abstract class AState : IState
    {
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
    }

    [Serializable]
    public class StateMachine<TLabel>
    {
        private static CommonState<TLabel>                s_Default = new CommonState<TLabel>(default);

        private TLabel                             m_CurrentLabel;
        private CoreLib.IState                     m_CurrentState = s_Default;
        public  TLabel                             PrevLabel { get; private set; }
        public  CoreLib.IState                     PrevState { get; private set; }
        private Dictionary<TLabel, CoreLib.IState> m_StateDictionary = new Dictionary<TLabel, CoreLib.IState>();
        [SerializeField]
        private bool                              m_AllowReEnter;
        [SerializeField]
        private bool                              m_LogStates;

        private StateChangeDelagate               m_OnStateChange;
        public delegate void StateChangeDelagate(CoreLib.IState current, CoreLib.IState next);
        
        public bool IsSet => m_CurrentState != s_Default;

        public ICollection<CoreLib.IState> States => m_StateDictionary.Values;
        public ICollection<TLabel> Labels => m_StateDictionary.Keys;

        public StateChangeDelagate OnStateChange
        {
            get => m_OnStateChange;
            set => m_OnStateChange = value;
        }

        // =======================================================================
        public interface IState : CoreLib.IState
        {
            TLabel               Label        { get; }
            StateMachine<TLabel> StateMachine { set; }
        }

        public abstract class StateObject : IState
        {
            public abstract TLabel Label { get; }
            public StateMachine<TLabel> StateMachine { get; set; }
            public bool IsActiveState => StateMachine.CurrentState().Equals(this);

            public virtual void OnEnter() { }
            public virtual void OnExit() { }
        }
        
        public abstract class StateObjectMonoBehaviour : MonoBehaviour, IState
        {
            public abstract TLabel Label { get; }
            public StateMachine<TLabel> StateMachine { get; set; }
            public bool IsActiveState => StateMachine.CurrentState().Equals(this);

            public virtual void Init(StateMachine<TLabel> stateMachine) => StateMachine = stateMachine;
            public virtual void OnEnter() { }
            public virtual void OnExit() { }
        }

        /// <summary> sets self as main state if enabled </summary>
        public abstract class StateObjectToggledMonoBehaviour : StateObjectMonoBehaviour
        {
            public override void OnEnter() { gameObject.SetActive(true); }

            public override void OnExit() { gameObject.SetActive(false); }

            protected virtual void OnEnable()
            {
                StateMachine.SetState(this);
            }
        }

        // =======================================================================
        public StateMachine()
        {
        }

        public StateMachine(params IState[] states)
        {
            foreach (var state in states)
                AddState(state);
        }
        
        public StateMachine<TLabel> AddState<TState>(TLabel label, TState state) where TState : CoreLib.IState
        {
            if (state is IState s)
                s.StateMachine = this;
            m_StateDictionary[label] = state;

            return this;
        }

        public StateMachine<TLabel> AddState<T>(T state) where T : IState
        {
            state.StateMachine = this;
            m_StateDictionary[state.Label] = state;

            return this;
        }

        public void RemoveState<T>(T state) where T : IState
        {
            m_StateDictionary.Remove(state.Label);
        }

        public StateMachine<TLabel> AddStates<T>(params T[] states) where T : IState
        {
            foreach (var state in states)
                AddState(state);

            return this;
        }

        public TStateType GetStateOfType<TStateType>()
        {
            return (TStateType)m_StateDictionary.Values.FirstOrDefault(n => n is TStateType);
        }

        public CoreLib.IState GetState(TLabel label)
        {
            if (m_StateDictionary.TryGetValue(label, out var state))
                return state;

            return default;
        }

        public bool IsCurrentState<T>() where T : class
        {
            return m_CurrentState.GetType() == typeof(T);
        }

        public TLabel CurrentLabel()
        {
            return m_CurrentLabel;
        }

        public CoreLib.IState CurrentState()
        {
            return m_CurrentState;
        }

        public T CurrentState<T>()
        {
            if (m_CurrentState is T state)
                return state;

            return default;
        }

        public bool TryGetState<T>(out T state) where T : IState
        {
            if (m_CurrentState is T s)
            {
                state = s;
                return true;
            }

            state = default;
            return false;
        }

        public void SetState<TStateType>() where TStateType : IState
        {
            var state = GetStateOfType<TStateType>();
            if (state != null)
                _setState(state);
        }

        public void SetState(TLabel label)
        {
            if (m_StateDictionary.TryGetValue(label, out var state))
                _setState(label, state);
        }
        
        public void SetState(IState state)
        {
            _setState(state);
        }
        
        public void SetStateOfType<T>()
        {
            if (GetStateOfType<T>() is IState state)
                _setState(state);
        }

        public void SetStateOfType(Type type)
        {
            var state = m_StateDictionary.FirstOrDefault(n => type.IsInstanceOfType(n.Value));
            if (state.Value != null)
                _setState(state.Key, state.Value);
        }

        /// <summary> Returns the current state name </summary>
        public override string ToString()
        {
            return m_CurrentState?.ToString() ?? string.Empty;
        }

        // =======================================================================
        /// <summary> Changes the state from the existing one to the given </summary>
        private void _setState(IState state)
        {
            _setState(state.Label, state);
        }

        private void _setState(TLabel label, CoreLib.IState state)
        {
            m_CurrentLabel = label;

            PrevState = m_CurrentState;
            PrevLabel = m_CurrentLabel;

            if (state.Equals(m_CurrentState))
            {
                // activate ReEnter
                if (m_AllowReEnter)
                {
                    m_CurrentState.OnExit();
                    m_CurrentState.OnEnter();
                }
                return;
            }

            // set new state
            m_CurrentState.OnExit();
            if (m_LogStates)
                Debug.Log($"{m_CurrentState} : <color=yellow>{state}</color>");
            m_OnStateChange?.Invoke(m_CurrentState, state);
            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }
    }
}