using System;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetParent : MonoBehaviour
    {
        public Vers<GameObject> _go;
        public bool             _onEnable;
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            gameObject.transform.SetParent(_go.Value.transform);
        }
    }
}