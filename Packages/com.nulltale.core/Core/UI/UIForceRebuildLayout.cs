using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIForceRebuildLayout : MonoBehaviour
    {
        public Mode _mode;

        // =======================================================================
        public enum Mode
        {
            OnStart,
            OnActivation,
            Manula
        }
        
        // =======================================================================
        private void Start()
        {
            if (_mode == Mode.OnStart)
                Invoke();
        }

        private void OnEnable()
        {
            if (_mode == Mode.OnActivation)
                Invoke();
        }

        private void OnDisable()
        {
            if (_mode == Mode.OnActivation)
                Invoke();
        }

        public void Invoke()
        {
            foreach (var lg in gameObject.GetComponentInParentAll<LayoutGroup>())
                LayoutRebuilder.ForceRebuildLayoutImmediate(lg.GetComponent<RectTransform>());
        }
    }
}