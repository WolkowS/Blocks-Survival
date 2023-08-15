using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class TransformMimic : MonoBehaviour
    {
        public Transform _target;

        // =======================================================================
        private void LateUpdate()
        {
            transform.position   = _target.transform.position;
            transform.rotation   = _target.transform.rotation;
            transform.localScale = _target.transform.localScale;
        }
    }
}