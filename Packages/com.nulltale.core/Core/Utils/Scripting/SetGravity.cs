using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetGravity : MonoBehaviour
    {
        public Vector2 _gravity;
        public bool    _onEnable;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            Physics2D.gravity = _gravity;
        }
    }
}