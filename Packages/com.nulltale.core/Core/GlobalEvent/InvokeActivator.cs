using UnityEngine;

namespace CoreLib.Events
{
    public class InvokeActivator : MonoBehaviour
    {
        public interface IHandle
        {
            void Invoke();
        }
    
        // =======================================================================
        private void OnEnable()
        {
            GetComponent<IHandle>().Invoke();
            gameObject.SetActive(false);
        }
    }
}