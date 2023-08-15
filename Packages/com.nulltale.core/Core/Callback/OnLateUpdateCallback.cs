using System;
using UnityEngine;

namespace CoreLib
{
    [AddComponentMenu("")]
    public class OnLateUpdateCallback : CallbackBase
    {
        public Action Action { get; set; }
	
        // =======================================================================
        private void LateUpdate()
        {
            Action.Invoke();
        }
    }
}