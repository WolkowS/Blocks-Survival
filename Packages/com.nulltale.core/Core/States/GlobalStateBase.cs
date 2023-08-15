using System;
using System.Collections;
using SoCreator;
using UnityEngine;

namespace CoreLib.States
{
    [SoCreate(true, true)]
    public abstract class GlobalStateBase : ScriptableObject
    {
        public abstract bool IsOpen { get; }

        public event Action OnOpen;
        public event Action OnClose;

        // =======================================================================
        public abstract void Open();
        public abstract void Close();

        [ContextMenu("Open")]
        public void OpenClick()
        {
            Open();
        }
        
        [ContextMenu("Close")]
        public void CloseClick()
        {
            Close();
        }

        public void Open(float duration)
        {
            if (duration <= 0f)
                return;

            Core.Instance.StartCoroutine(_open(new WaitForSeconds(duration)));

            IEnumerator _open(object interval)
            {
                Open();
                yield return interval;
                Close();
            }
        }

        public void Open(float duration, bool ignoreTimeScale)
        {
            if (duration <= 0f)
                return;

            Core.Instance.StartCoroutine(_open(ignoreTimeScale ? new WaitForSecondsRealtime(duration) : new WaitForSeconds(duration)));

            IEnumerator _open(object interval)
            {
                Open();
                yield return interval;
                Close();
            }
        }

        internal void Init()
        {
            OnClose = null;
            OnOpen  = null;
            
            _init();
        }

        protected virtual void _init() { }

        protected void InvokeOnOpen()
        {
            OnOpen?.Invoke();
        }
        
        protected void InvokeOnClose()
        {
            OnClose?.Invoke();
        }
    }
}