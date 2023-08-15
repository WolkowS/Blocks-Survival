using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Distance : MonoBehaviour
    {
        public Vers<GameObject>  _go;
        [SerializeField] [HideInInspector]
        private Ref<float>       _value;
        public UnityEvent<float> _onUpdate;
        
        // =======================================================================
        private void Update()
        {
            _value.Value = Vector3.Distance(transform.position, _go.Value.transform.position);
            _onUpdate.Invoke(_value.Value);
        }
    }
}