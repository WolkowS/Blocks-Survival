using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib
{
    public class OnDirectorEvaluate : MonoBehaviour
    {
        private PlayableDirector _director;

        // =======================================================================
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }
        
        public void Time(float time)
        {
            _director.time = time;
            _director.Evaluate();
        }
        
        public void Scale(float scale)
        {
            _director.time = scale * _director.duration;
            _director.Evaluate();
        }
    }
}