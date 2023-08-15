using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CoreLib
{
    public class OnDirectorStop : MonoBehaviour
    {
        public UnityEvent m_Event;

        // =======================================================================
        private void Awake()
        {
            GetComponent<PlayableDirector>().stopped += director => m_Event.Invoke();
        }
    }
}