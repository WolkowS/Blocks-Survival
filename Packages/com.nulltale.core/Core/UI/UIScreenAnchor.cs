using System;
using UnityEngine;

namespace CoreLib
{
    [DefaultExecutionOrder(100)]
    public class UIScreenAnchor : MonoBehaviour
    {
        private Canvas m_Canvas;
        public Vector3 Position { get; private set; }

        // =======================================================================
        protected void OnEnable()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            var pos = transform.position;

            switch (m_Canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                {
                    var cam = Core.Camera;
                    Position = new Vector3(pos.x / cam.pixelWidth, pos.y / cam.pixelHeight, pos.z / transform.lossyScale.z);
                } break;
                case RenderMode.ScreenSpaceCamera:
                {
                    var cam       = m_Canvas.worldCamera;
                    var screenPos = cam.WorldToScreenPoint(pos);
                    Position = new Vector3(screenPos.x / cam.pixelWidth, screenPos.y / cam.pixelHeight, pos.z / transform.lossyScale.z);
                } break;
                case RenderMode.WorldSpace:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}