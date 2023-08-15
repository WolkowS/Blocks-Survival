using UnityEngine;
using UnityEngine.Video;

namespace CoreLib
{
    [DefaultExecutionOrder(-10000)]
    public class SetMainCamera : MonoBehaviour
    {
        private void Awake()
        {
            if (TryGetComponent<Canvas>(out var canvas))
                canvas.worldCamera = Core.Camera;
            
            if (TryGetComponent<VideoPlayer>(out var player))
                player.targetCamera = Core.Camera;
        }
    }
}