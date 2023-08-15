using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class InCameraView : MonoBehaviour
    {
        public float            _boundsScale;
        public UnityEvent<bool> _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            var cam   = Camera.main;
            var ortho = cam.orthographicSize * _boundsScale;
            var screen = new Rect(-ortho * cam.aspect + cam.transform.position.x,
                                  -ortho + cam.transform.position.y,
                                  ortho * cam.aspect * 2f, ortho * 2f);
            
            _onInvoke.Invoke(screen.Contains(transform.position));
        }
    }
}