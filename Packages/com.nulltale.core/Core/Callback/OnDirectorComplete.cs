using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace CoreLib
{
    public class OnDirectorComplete : MonoBehaviour
    {
        private PlayableDirector _director;
        public  UnityEvent       _onInvoke;
        private bool             _triggered;

        // =======================================================================
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }

        private void Update()
        {
            switch (_director.extrapolationMode)
            {
                case DirectorWrapMode.Hold:
                {
                    if (_director.time == _director.duration)
                    {
                        if (_triggered == false)
                            _onInvoke.Invoke();
                        
                        _triggered = true;
                    }
                    else
                    {
                        _triggered = false;
                    }
                } break;
                case DirectorWrapMode.Loop:
                    break;
                case DirectorWrapMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}