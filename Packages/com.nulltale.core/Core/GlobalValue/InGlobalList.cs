using UnityEngine;

namespace CoreLib.Values
{
    public class InGlobalList<T> : MonoBehaviour
    {
        public GlobalList<T> _list;
        
        // =======================================================================
        private void OnEnable()
        {
            _list.Value.Add(GetComponentInParent<T>());
        }

        private void OnDisable()
        {
            _list.Value.Remove(GetComponentInParent<T>(true));
        }
    }

    public class InGlobalList : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _target;
        public GlobalListGo _list;
        
        // =======================================================================
        private void OnEnable()
        {
            _list.Add(_target.Enabled ? _target.Value.Value : gameObject);
        }
        
        public void Release()
        {
            _list.Remove(_target.Enabled ? _target.Value.Value : gameObject);
        }

        private void OnDisable()
        {
            Release();
        }
    }
}