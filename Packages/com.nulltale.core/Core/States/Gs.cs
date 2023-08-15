using System;
using System.Collections;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib.States
{
    [SoCreate(true, priority: -1000)]
    public class Gs : GlobalStateBase
    {
        [SerializeField]
        private int m_CloseDelay = 3;

        [ShowNonSerializedField]
        private int m_OpenCounter;

        private Coroutine m_CloseCoroutine;

        [ShowNonSerializedField]
        protected bool    m_IsOpen;
        public override bool IsOpen => m_IsOpen;

        [ResizableTextArea]
        public string _note;

        // =======================================================================
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public override void Open()
        {
#if UNITY_EDITOR
            if (Core.Instance == null)
                return;
#endif
            if (++ m_OpenCounter == 1)
            {
                if (m_CloseCoroutine != null)
                    Core.Instance.StopCoroutine(m_CloseCoroutine);

                if (m_IsOpen == false)
                {
                    m_IsOpen = true;
                    InvokeOnOpen();
                }
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public override void Close()
        {
#if UNITY_EDITOR
            if (Core.Instance == null)
                return;
#endif
            if (-- m_OpenCounter == 0)
                m_CloseCoroutine = Core.Instance.StartCoroutine(_close(m_CloseDelay));

            // ===================================
            IEnumerator _close(int frames)
            {
                while (frames -- > 0)
                    yield return null;

                m_CloseCoroutine = null;
                m_IsOpen = false;
                InvokeOnClose();
            }
        }

        protected override void _init()
        {
            m_OpenCounter = 0;
            m_IsOpen = false;
            m_CloseCoroutine = null;
        }
    }
}