using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CoreLib.Scripting
{
    public class OnLeftClick : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public  UnityEvent _onInvoke;
        private bool       _drag;
        
        // =======================================================================
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_drag)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            _onInvoke.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _drag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _drag = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        private void OnDisable()
        {
            _drag = false;
        }
    }
}