using Cinemachine;
using UnityEngine;

namespace CoreLib.Cinemachine
{
    [AddComponentMenu("")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CmConfiner2D : CinemachineExtension
    {
        public Collider2D m_Bounds;

        // =======================================================================
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Body)
                return;

            var bounds = m_Bounds.bounds.ToRectXY();

            var aspectRatio = state.Lens.Aspect;
            var ortho       = state.Lens.OrthographicSize * 2f;
            var orthoSize   = new Vector2(ortho * aspectRatio, ortho);
            var screenRect  = new Rect(state.CorrectedPosition.To2DXY() - orthoSize * 0.5f, orthoSize);

            // if bounds less correct camera to left bottom point, else clamp by bounds
            var correction = Vector3.zero;
            if (bounds.width > screenRect.width)
            {
                correction.x += (bounds.min.x - screenRect.min.x).ClampPositive();
                correction.x += (bounds.max.x - screenRect.max.x).ClampNegative();
            }
            else
            {
                correction.x += bounds.min.x - screenRect.min.x;
            }
            if (bounds.height > screenRect.height)
            {
                correction.y += (bounds.min.y - screenRect.min.y).ClampPositive();
                correction.y += (bounds.max.y - screenRect.max.y).ClampNegative();
            }
            else
            {
                correction.y += bounds.min.y - screenRect.min.y;
            }
                
            state.PositionCorrection += correction;
        }
    }
}