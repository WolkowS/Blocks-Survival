using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.CommandSystem
{
    public interface ICommand
    {
        IEnumerator Run();
    }

    public abstract class CommandBase : ICommand
    {
        public virtual IEnumerator Run()
        {
            yield return _run();
        }

        protected abstract IEnumerator _run();
    }

    [Serializable]
    public class CommandCoroutine : CommandBase
    {
        [SerializeField]
        private CoroutineWrapper    m_Coroutine;

        protected override IEnumerator _run()
        {
            yield return _listener();
        }

        private IEnumerator _listener()
        {
            m_Coroutine.Start();
            while (m_Coroutine.IsRunning)
                yield return null;
        }
    }

    [Serializable]
    public class CommandAction : CommandBase
    {
        private Action    m_Action;

        public CommandAction(Action action)
        {
            m_Action = action;
        }

        protected override IEnumerator _run()
        {
            m_Action?.Invoke();
            yield break;
        }
    }

    [Serializable]
    public class CommandYield : CommandBase
    {
        private object m_Yield;

        public CommandYield(object toYield)
        {
            m_Yield = toYield;
        }

        protected override IEnumerator _run()
        {
            yield return m_Yield;
        }
    }

    [Serializable]
    public class CommandWhile : CommandBase
    {
        private Func<bool> m_While;

        public CommandWhile(Func<bool> waitWhile)
        {
            m_While = waitWhile;
        }

        protected override IEnumerator _run()
        {
            while (m_While.Invoke())
                yield return null;
        }
    }

    [Serializable]
    public class CommandWait : CommandBase
    {
        [SerializeField]
        private float       m_Delay;
        public float        Delay
        {
            get => m_Delay;
            set => m_Delay = value;
        }

        // =======================================================================
        protected override IEnumerator _run()
        {
            yield return new WaitForSeconds(m_Delay);
        }
    }

    [Serializable]
    public class CommandLog : CommandBase
    {
        [SerializeField]
        private string    m_Log;

        public string Log
        {
            get => m_Log;
            set => m_Log = value;
        }

        protected override IEnumerator _run()
        {
            Debug.Log(m_Log);
            yield break;
        }
    }

    [Serializable]
    public class CommandSequence : ICommand
    {
        [SerializeField, SerializeReference, ClassReference]
        private List<ICommand>          m_Sequence = new List<ICommand>();
        
        // =======================================================================
        public IEnumerator Run()
        {
            foreach (var command in m_Sequence)
                yield return command.Run();
        }
    }
}