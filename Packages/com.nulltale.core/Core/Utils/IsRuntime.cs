using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class IsRuntime : MonoBehaviour
    {
        public UnityEvent _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            if (Application.isEditor == false)
                _onInvoke.Invoke();
        }
    }
}