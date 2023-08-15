using UnityEngine;

namespace CoreLib.Module
{
    public abstract class PointerIdActivator : MonoBehaviour
    {
        public PointerId _id;
        private bool    _stateLock;
        
        // =======================================================================
        protected void _take()
        {
            if (_stateLock)
                return;

            _stateLock = true;
            _id._state ++;
        }

        protected void _release()
        {
            if (_stateLock == false)
                return;

            _stateLock = false;
            _id._state --;
        }
        
        private void OnDisable()
        {
            _release();
        }
    }
}