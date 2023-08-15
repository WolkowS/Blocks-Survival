using UnityEngine;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    [DefaultExecutionOrder(100)]
    public class ForcePosition : MonoBehaviour
    {
        public GameObject _target;
        public bool       _inEditor;

        private Vector3 _posPrev;
        
        // =======================================================================
        private void Update()
        {
            var pos = transform.position;
            if (_posPrev == pos)
                return;
#if UNITY_EDITOR
            if (Application.isPlaying == false && _inEditor == false)
                return;
#endif
            _target.transform.position = transform.position;
            _posPrev = pos;
        }
    }
}