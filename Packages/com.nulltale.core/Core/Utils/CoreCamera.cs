using UnityEngine;

namespace CoreLib
{
    [DefaultExecutionOrder(-10000)]
    [RequireComponent(typeof(Camera))]
    public class CoreCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            Core.Camera = GetComponent<Camera>();
        }
    }
}