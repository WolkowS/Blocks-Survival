using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class OrientedSpriteBillboard : MonoBehaviour
    {
        // =======================================================================
        private void OnWillRenderObject()
        {
            // set rotation to the target
            if (Core.Camera.orthographic)
                transform.forward = Core.Camera.transform.forward;
            else
                transform.LookAt(Core.Camera.transform);
        }
    }
}