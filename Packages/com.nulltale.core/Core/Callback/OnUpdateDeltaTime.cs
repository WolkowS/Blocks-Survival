using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public sealed class OnUpdateDeltaTime : MonoBehaviour
    {
        public UnityEvent<float> _action;
	
        // =======================================================================
        private void Update()
        {
            _action.Invoke(Time.deltaTime);
        }
    }
}