using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.SceneManagement
{
    public class Transition : MonoBehaviour, INotificationReceiver
    {
        [SerializeField]
        private SignalAsset       m_Signal;
        [SerializeField]
        private bool              m_Unscaled = true;

        private PlayableDirector  m_Director;
        private Func<IEnumerator> m_Transition;
        private Func<IEnumerator> m_PreTransition;
        private Func<IEnumerator> m_PostTransition;

        // =======================================================================
        private void Awake()
        {
            m_Director = GetComponent<PlayableDirector>();
            m_Director.extrapolationMode = DirectorWrapMode.None;
            m_Director.playOnAwake = false;
            m_Director.stopped += _ => Destroy(gameObject);

            if (m_Unscaled)
                m_Director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, Core.Instance.m_BufferScene);
        }

        public void Play(Func<IEnumerator> onTransition, Func<IEnumerator> preTransition = null, Func<IEnumerator> postTransition = null)
        {
            // setup values, play, open state
            m_Transition     = onTransition;
            m_PreTransition  = preTransition;
            m_PostTransition = postTransition;

            m_Director.Play();
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitter signalEmitter && m_Signal == signalEmitter.asset)
            {
                signalEmitter.emitOnce = true;
                StartCoroutine(_transitionWrapper());
            }

            // -----------------------------------------------------------------------
            IEnumerator _transitionWrapper()
            {
                // start transition
                var rootplayable = m_Director.playableGraph.GetRootPlayable(0);
                var initialSpeed = rootplayable.GetSpeed();
                rootplayable.SetSpeed(0d);

                m_Director.time = signalEmitter.time;
                m_Director.Evaluate();

                if (m_PreTransition != null)
                    yield return m_PreTransition.Invoke();

                yield return m_Transition.Invoke();

                if (m_PostTransition != null)
                    yield return m_PostTransition.Invoke();

                m_Director.playableGraph.GetRootPlayable(0).SetSpeed(initialSpeed);
            }
        }
    }
}