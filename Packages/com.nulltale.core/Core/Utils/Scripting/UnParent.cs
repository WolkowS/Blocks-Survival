using System.Linq;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class UnParent : MonoBehaviour
    {
        public bool _onEnable;
        public bool _up;
        public bool _self = true;
        public bool _childern;
        public bool _setActive;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }
        
        public void Invoke()
        {
            if (_self)
            {
                _unparent(gameObject);
            }
            
            if (_childern)
            {
                foreach (var child in gameObject.GetChildren().ToArray())
                    _unparent(child.gameObject);
            }
        }
        
        private void _unparent(GameObject go)
        {
            var pos = go.transform.position;
            go.transform.SetParent(_up ? transform.parent.parent : null, false);
            go.transform.position = pos;
            
            if (_setActive)
                go.SetActive(true);
        }
    }
}