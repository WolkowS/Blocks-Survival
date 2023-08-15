using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SceneState : MonoBehaviour
    {
        private const int k_CloseDelayFrames = 3;

        public IdAsset m_ID;

        [SerializeField] [ReadOnly]
        private int m_OpenCounter;

        private Coroutine m_CloseCoroutine;

        [SerializeField] [ReadOnly]
        protected bool m_IsOpen;
        public bool IsOpen => m_IsOpen;

        public event Action OnOpen;
        public event Action OnClose;

        // =======================================================================
        private void Awake()
        {
            SceneManager.Instance.m_SceneData[gameObject.scene.handle].m_States.Add(m_ID, this);
            gameObject.SetActive(IsOpen);
        }

        public void Open()
        {
            if (++ m_OpenCounter == 1)
            {
                if (m_CloseCoroutine != null)
                    StopCoroutine(m_CloseCoroutine);

                if (m_IsOpen == false)
                {
                    m_IsOpen = true;
                    gameObject.SetActive(true);
                    OnOpen?.Invoke();
                }
            }
        }

        public void Close()
        {
            if (-- m_OpenCounter == 0)
                m_CloseCoroutine = StartCoroutine(_close(k_CloseDelayFrames));

            // ===================================
            IEnumerator _close(int frames)
            {
                while (frames -- > 0)
                    yield return null;

                m_CloseCoroutine = null;
                m_IsOpen         = false;
                gameObject.SetActive(false);
                OnClose?.Invoke();
            }
        }
    }
}