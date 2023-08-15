using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraMimic : MonoBehaviour
    {
        public  Camera _target;
        private Camera _camera;
        public  bool   _position;
        public  bool   _rotation;
        public  bool   _projection;
        public  bool   _viewport;

        // =======================================================================
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (_position)
                transform.position = _target.transform.position;
            
            if (_rotation)
                transform.rotation = _target.transform.rotation;
            
            if (_projection)
            {
                _camera.orthographic     = _target.orthographic;
                _camera.orthographicSize = _target.orthographicSize;
                _camera.farClipPlane     = _target.farClipPlane;
                _camera.nearClipPlane    = _target.nearClipPlane;
            }
            
            if (_viewport)
            {
                _camera.rect = _target.rect;
            }
        }
    }
}