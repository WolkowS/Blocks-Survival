using UnityEngine;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    [DefaultExecutionOrder(100)]
    public class ForceScale : MonoBehaviour
    {
        public GameObject _target;
        public bool       _inEditor;

        // =======================================================================
        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && _inEditor == false)
                return;
#endif
            _target.transform.localScale = transform.localScale;
        }
    }
}