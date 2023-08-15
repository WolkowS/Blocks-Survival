using System;
using System.Collections;
using System.Collections.Generic;
using CoreLib.Values;
using JetBrains.Annotations;
using SoCreator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.Commands
{
    [SoCreate]
    public abstract class Cmd : ScriptableObject
    {
        public virtual bool RuntimeOnly => true;

        public Handle _handle;
        
        // =======================================================================
        public class Handle : CustomYieldInstruction
        {
            internal        bool   _isOpen;
            internal        Action _continuation;
            public          bool   IsOpen      => _isOpen;
            public override bool   keepWaiting => _isOpen;

            // =======================================================================
            public void Complete()
            {   
                _isOpen = false;
                _continuation?.Invoke();
                StaticPool<Handle>.Release(this);
            }
        }

        public interface IArgsCall
        {
            internal void Invoke(Object args);
        }
        
        public interface IVoidCall
        {
            internal void Invoke();
        }
        
        // =======================================================================
        protected Handle CreateHandle()
        {
            var handle = StaticPool<Handle>.Get();
            handle._isOpen = true;
            
            _handle = handle;
            return handle;
        }
        
        protected Handle CreateHandle(Func<bool> keepWait)
        {
            var handle = StaticPool<Handle>.Get();
            handle._isOpen = true;
            
            Core.Instance.StartCoroutine(_handleContinuation());
            
            _handle = handle;
            return handle;

            // -----------------------------------------------------------------------
            IEnumerator _handleContinuation()
            {
                while (keepWait())
                    yield return null;
                
                handle.Complete();
            }
        }
    }

    public abstract class CmdInvoke<T> : Cmd, Cmd.IArgsCall 
        where T : Object
    {
        void IArgsCall.Invoke(Object args) => Invoke((T)args);
        public abstract void Invoke(T args);
    }
    
    public abstract class CmdInvoke : Cmd, Cmd.IVoidCall 
    {
        void IVoidCall.Invoke() => Invoke();

        public abstract void Invoke();
    }

    public abstract class CmdInvokeGo : CmdInvoke<GameObject>
    {
    }
}