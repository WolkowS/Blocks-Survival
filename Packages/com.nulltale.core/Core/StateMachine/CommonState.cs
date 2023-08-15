using System;

namespace CoreLib
{
    public class CommonState<TLabel> : StateMachine<TLabel>.IState
    {
        public TLabel               Label        { get; }
        public StateMachine<TLabel> StateMachine { get; set; }

        internal Action m_OnEnter;
        internal Action m_OnExit;
        internal Action m_OnInvoke;

        // =======================================================================
        public CommonState(TLabel label)
        {
            Label = label;
        }

        public void OnInvoke()
        {
            m_OnInvoke?.Invoke();
        }

        public void OnEnter()
        {
            m_OnEnter?.Invoke();
        }

        public void OnExit()
        {
            m_OnExit?.Invoke();
        }
    }
    
    public class StateBuilder<TLabel, TState>
        where TState : StateMachine<TLabel>.IState
    {
        protected StateMachine<TLabel> m_StateMachine;
        protected TState               m_State;

        // =======================================================================
        public StateBuilder(StateMachine<TLabel> stateMachine, TState state)
        {
            m_StateMachine = stateMachine;
            m_State        = state;
        }

        public StateMachine<TLabel> Build()
        {
            m_StateMachine.AddState(m_State);
            return m_StateMachine;
        }
    }

    public sealed class CommonStateBuilder<TLabel> : StateBuilder<TLabel, CommonState<TLabel>> 
    {
        public CommonStateBuilder(StateMachine<TLabel> stateMachine, CommonState<TLabel> state) : base(stateMachine, state) { }

        public CommonStateBuilder<TLabel> OnEnter(Action action)
        {
            m_State.m_OnEnter = action;
            return this;
        }
        
        public CommonStateBuilder<TLabel> OnExit(Action action)
        {
            m_State.m_OnExit = action;
            return this;
        }

        public CommonStateBuilder<TLabel> OnInvoke(Action action)
        {
            m_State.m_OnInvoke = action;
            return this;
        }
    }

    public static partial class StateMachineExtensions
    {
        public static CommonStateBuilder<TLabel> AddStateCommon<TLabel>(this StateMachine<TLabel> sm, TLabel label)
        {
            return new CommonStateBuilder<TLabel>(sm, new CommonState<TLabel>(label));
        }
    }
}