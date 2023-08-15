using UnityEngine.EventSystems;

namespace CoreLib.Module
{
    public class PointerPoint : PointerActivator, IPointerEnterHandler, IPointerExitHandler
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_Point._id;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _take();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _release();
        }
    }
    
    public class PointerView : PointerActivator, IPointerEnterHandler, IPointerExitHandler
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_Point._id;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _take();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _release();
        }
    }
}