using UnityEngine;

namespace CoreLib.Values
{
    [ExecuteAlways] [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder + 1)]
    public class SetGo : MonoBehaviour
    {
        public GvGo _value;
        
        // =======================================================================
        private void OnEnable()
        {
            _value.Value = gameObject;
        }
        
        public void Invoke()
        {
            _value.Value = gameObject;
        }
    }
}