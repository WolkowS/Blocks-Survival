using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    [Serializable]
    public class TinyFSM<TLabel>
    {
        public static State s_Default = new State();

        public Dictionary<TLabel, State> _states = new Dictionary<TLabel, State>();

        public TLabel Label
        {
            get;
            private set;
        }
        
        public State Current
        {
            get;
            private set;
        }
        
        // =======================================================================
        public class State
        {
            public virtual void OnEnter() { }
            public virtual void OnUpdate() { }
            public virtual void OnExit() { }
        }

        private class Generic : State
        {
            public Action _onEnter;
            public Action _onUpdate;
            public Action _onExit;
            
            // =======================================================================
            public override void OnEnter()
            {
                _onEnter?.Invoke();
            }

            public override void OnUpdate()
            {
                _onUpdate?.Invoke();
            }

            public override void OnExit()
            {
                _onExit?.Invoke();
            }
        }

        // =======================================================================
        public TinyFSM()
        {
            Current = s_Default;
        }

        public override string ToString()
        {
            var kvp =_states.FirstOrDefault(n => Equals(n.Value, Current));
            return $"{kvp.Key} : {kvp.Value}";
        }

        internal TLabel CurrentLabel()
        {
            return Label;
        }

        public void Update()
        {
            Current.OnUpdate();
        }
        
        public TinyFSM<TLabel> Add(TLabel label, Action onEnter = null, Action onExit = null, Action onUpdate = null)
        {
            return Add(label, new Generic() { _onEnter = onEnter, _onUpdate = onUpdate, _onExit = onExit });
        }

        public TinyFSM<TLabel> Add(TLabel label, State state)
        {
            _states.Add(label, state);
            return this;
        }
        
        public void Set(State state)
        {
            Current.OnExit();
            Current = state;
            Current.OnEnter();
        }
        
        public void Set(TLabel label)
        {
            Label = label;
            Set(_states[label]);
        }

        public void Clear()
        {
            _states.Clear();
        }
    }
}