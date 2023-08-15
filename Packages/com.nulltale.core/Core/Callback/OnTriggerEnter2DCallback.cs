using System;
using UnityEngine;

namespace CoreLib
{
    [AddComponentMenu("")]
    public class OnTriggerEnter2DCallback : CallbackBase
    {
        public Action<Collider2D> Action;

        // =======================================================================
        private void OnTriggerEnter2D(Collider2D other)
        {
            Action.Invoke(other);
        }
    }
}