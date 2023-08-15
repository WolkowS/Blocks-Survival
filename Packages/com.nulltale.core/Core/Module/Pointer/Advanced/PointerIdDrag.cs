using UnityEngine.EventSystems;

namespace CoreLib.Module
{
    public class PointerIdDrag : PointerIdActivator, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            _take();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _release();
        }
    }
}