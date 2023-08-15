using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [ExecuteAlways]
    public sealed class OnUpdate : MonoBehaviour
    {
        public UnityEvent _action;
	
        // =======================================================================
        private void Update()
        {
            _action.Invoke();
        }
    }
}