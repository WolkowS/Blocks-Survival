using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CoreLib.Scriptiong
{
    public class DirectorProgress : MonoBehaviour
    {
        private PlayableDirector _director;
        private float            _current;
        
        public UnityEvent<float> _onChanged;
        
        // =======================================================================
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _current  = -1f;
        }

        private void Update()
        {
            var current = (float)(_director.time / _director.duration);
            if (current == _current)
                return;
            
            _current = current;
            _onChanged.Invoke(_current);
        }
    }
}