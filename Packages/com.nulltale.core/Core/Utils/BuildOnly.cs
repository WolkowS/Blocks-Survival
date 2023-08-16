using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class BuildOnly : MonoBehaviour
    {
        public UnityEvent _onInvoke;
        
        // =======================================================================
        public void OnEnable()
        {
            if (Application.isEditor == false)
                _onInvoke.Invoke();
        }
    }
}