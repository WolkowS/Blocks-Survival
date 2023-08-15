using UnityEngine.EventSystems;

namespace CoreLib.Module
{
    public class PointerHover : PointerActivator, IPointerEnterHandler, IPointerExitHandler
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_Hover._id;

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