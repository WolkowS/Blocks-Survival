using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    public class StateMachineLogic<TState, TTrigger>
    {
        private SortedCollection<Transition>                     m_AnyTransitions = new SortedCollection<Transition>(Transition.PriorityComparer);
        private Dictionary<TState, SortedCollection<Transition>> m_Transitions    = new Dictionary<TState, SortedCollection<Transition>>();
        private HashSet<TTrigger>                                m_Triggers       = new HashSet<TTrigger>();

        // =======================================================================
        public class Transition
        {
            public int                                 Priority;
            public StateMachineLogic<TState, TTrigger> Owner;
            public TState                              From;
            public TState                              To;
            public Func<bool>                          Condition;

            // =======================================================================
            private sealed class PriorityRelationalComparer : IComparer<Transition>
            {
                public int Compare(Transition x, Transition y)
                {
                    return x.Priority.CompareTo(y.Priority);
                }
            }

            internal static IComparer<Transition> PriorityComparer { get; } = new PriorityRelationalComparer();
        }

        // =======================================================================
        public void Run(StateMachine<TState> stateMachine)
        {
            // try transition from any first
            foreach (var transition in m_AnyTransitions)
            {
                if (transition.Condition())
                {
                    stateMachine.SetState(transition.To);
                    return;
                }
            }

            // apply state transition or any state transition
            if (m_Transitions.TryGetValue(stateMachine.CurrentLabel(), out var transitions))
            {
                var transition = transitions.FirstOrDefault(n => n.Condition());
                if (transition != null)
                {
                    stateMachine.SetState(transition.To);
                    return;
                }
            }
        }

        public void Trigger(TTrigger id)
        {
            m_Triggers.Add(id);
        }

        public StateMachineLogic<TState, TTrigger> AddTrigger(TState from, TState to, TTrigger id)
        {
            AddTransition(from, to, () => m_Triggers.Remove(id));
            return this;
        }

        public StateMachineLogic<TState, TTrigger> AddTrigger(TState to, TTrigger id)
        {
            AddTransition(to, () => m_Triggers.Remove(id));
            return this;
        }

        public StateMachineLogic<TState, TTrigger> AddTransition(TState from, TState to, Func<bool> condition)
        {
            Add(new Transition()
            {
                Owner     = this,
                From      = from,
                To        = to,
                Condition = condition,
            });

            return this;
        }

        public StateMachineLogic<TState, TTrigger> AddTransition(TState to, Func<bool> condition)
        {
            m_AnyTransitions.Add(new Transition()
            {
                Owner     = this,
                To        = to,
                Condition = condition,
            });

            return this;
        }

        public void Add(Transition transition)
        {
            if (m_Transitions.TryGetValue(transition.From, out var transitions) == false)
            {
                transitions = new SortedCollection<Transition>(Transition.PriorityComparer);
                m_Transitions.Add(transition.From, transitions);
            }

            transitions.Add(transition);
        }
    }
}