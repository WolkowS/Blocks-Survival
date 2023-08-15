using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraLayerMask : MonoBehaviour
    {
        public  LayerMask _ignore;
        private Camera    _camera;

        // =======================================================================
        private void Awake()
        {
            if (Application.isPlaying)
                enabled = false;
            
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            _camera.cullingMask = (-1 & ~_ignore.value);
        }
    }
}