using UnityEngine;

namespace CoreLib
{
    public class ObjectState : MonoBehaviour
    {
        public bool _isActive;
        
        // =======================================================================
        public void Apply()
        {
            gameObject.SetActive(_isActive);
        }
        
        public void ApplyRuntime()
        {
            gameObject.SetActive(_isActive);
            Destroy(this);
        }
    }
}