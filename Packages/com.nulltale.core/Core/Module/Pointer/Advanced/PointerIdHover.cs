using UnityEngine.EventSystems;

namespace CoreLib.Module
{
    public class PointerIdHover : PointerIdActivator, IPointerEnterHandler, IPointerExitHandler
    {
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