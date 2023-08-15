using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class SetRotation : MonoBehaviour
    {
        public Vers<float> _degrees;
        
        public bool _onEnable;

        public UnityEvent<Quaternion> _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            _onInvoke.Invoke(Quaternion.AngleAxis(_degrees.Value, Vector3.forward));
        }
    }
}