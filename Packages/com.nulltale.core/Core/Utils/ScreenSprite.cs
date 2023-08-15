using UnityEngine;
// ReSharper disable All

namespace CoreLib
{
    [ExecuteAlways]
    [DefaultExecutionOrder(1000)]
    public class ScreenSprite : MonoBehaviour
    {
        public bool            m_Scale;
        public Optional<float> m_Distance;
        
        // =======================================================================
        private void Update()
        {
            var camera = Core.Camera;

            var distance = m_Distance.Enabled ? m_Distance.Value : camera.nearClipPlane + 0.0001f;
            var frustumHeight = camera.orthographic 
                ? camera.orthographicSize * 2f 
                : 2f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var frustumWidth  = frustumHeight * camera.aspect;

            transform.SetPositionAndRotation(camera.transform.position + camera.transform.forward * (distance), camera.transform.rotation);
            if (m_Scale)
                transform.localScale = new Vector3(frustumWidth, frustumHeight, 1f);
        }
    }
}