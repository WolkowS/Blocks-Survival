using System;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class ActivateNextSbling : MonoBehaviour
    {
        public bool _onEnable;
        public bool _onDisable;

        // =======================================================================
        public void Invoke()
        {
            var index = transform.GetSiblingIndex();
            if (transform.parent.childCount - 1 == index)
                return;
            
            gameObject.SetActive(false);
            transform.parent.GetChild(index + 1).gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (_onEnable)
                Invoke();   
        }

        private void OnDisable()
        {
            if (_onDisable)
                Invoke();
        }
    }
}