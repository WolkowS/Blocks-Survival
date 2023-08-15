using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Events
{
    public class InvokeGeString : MonoBehaviour
    {
        [ResizableTextArea]
        public string _data;
        
        public GeString _event;
        
        public bool _onEnable;

        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();    
        }
        
        public void Invoke()
        {
            _event.Invoke(_data);
        }
    }
}