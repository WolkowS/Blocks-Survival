using UnityEngine.EventSystems;

namespace CoreLib.Module
{
    public class PointerDrag : PointerActivator, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_Drag._id;
        
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