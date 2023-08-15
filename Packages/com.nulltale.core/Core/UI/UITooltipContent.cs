using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreLib
{
    public class UITooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [ResizableTextArea]
        public string _content;
        public Optional<Transform> _anchor;

        private bool  _active;
        
        // =======================================================================
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_active)
                return;
            
            _active = true;
            UITooltip.Instance.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_active == false)
                return;
            
            _active = false;
            UITooltip.Instance.Hide();
        }

        private void OnDisable()
        {
            if (_active == false)
                return;
            
            _active = false;
            UITooltip.Instance.Hide();
        }
    }
}