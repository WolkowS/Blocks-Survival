using CoreLib.Values;
using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    [DefaultExecutionOrder(1)]
    public class Follow : MonoBehaviour
    {
        public Vers<GameObject> _target;

        // =======================================================================
        private void Update()
        {
            transform.position = _target.Value.transform.position;
        }
        
        public void Invoke()
        {
            transform.position = _target.Value.transform.position;
        }
    }
}