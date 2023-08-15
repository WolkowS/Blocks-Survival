using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    public class OnGlobalListGo : MonoBehaviour
    {
        public GlobalListGo _list;

        public UnityEvent<GameObject> _onAdd;
        public UnityEvent<GameObject> _onRemove;
        
        // =======================================================================
        private void OnEnable()
        {
            _list.OnAdd    += _onAdd.Invoke;
            _list.OnRemove += _onRemove.Invoke;
        }
        
        private void OnDisable()
        {
            _list.OnAdd    -= _onAdd.Invoke;
            _list.OnRemove -= _onRemove.Invoke;
        }
    }
}