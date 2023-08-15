using UnityEngine;

namespace CoreLib
{
    public class TransformLocalMimic : MonoBehaviour
    {
        public Transform _target;

        // =======================================================================
        private void LateUpdate()
        {
            transform.localPosition = _target.transform.localPosition;
            transform.localRotation = _target.transform.localRotation;
            transform.localScale    = _target.transform.localScale;
        }
    }
}