using System;
using UnityEngine;

namespace CoreLib.Module
{
    public abstract class PointerActivator : MonoBehaviour
    {
        private PointerId _id;
        private bool     _stateLock;
        
        // =======================================================================
        protected abstract PointerId _reloveId();

        private void Awake()
        {
            _id = _reloveId();
        }

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