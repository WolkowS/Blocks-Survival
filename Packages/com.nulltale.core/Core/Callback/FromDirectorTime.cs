using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CoreLib
{
    public class FromDirectorTime : MonoBehaviour
    {
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            _onInvoke.Invoke((float)GetComponent<PlayableDirector>().time);
        }
    }
}