using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetPositionRandomScreen : MonoBehaviour
    {
        public Optional<float> _padding;
        public bool _onEnable;

        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }
        
        public void Invoke()
        {
            transform.position = Core.Camera
                                     .OrthographicBounds()
                                     .ToRectXY()
                                     .Inflate(_padding.GetValueOrDefault())
                                     .RandomPoint()
                                     .To3DXY();
        }
    }
}