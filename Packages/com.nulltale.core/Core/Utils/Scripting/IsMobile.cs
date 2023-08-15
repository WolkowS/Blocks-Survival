using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IsMobile : MonoBehaviour
    {
        public UnityEvent _onInvoke;
        
        // =======================================================================
        private void Start()
        {
            if (Application.isMobilePlatform)
                _onInvoke.Invoke();
        }
    }
}