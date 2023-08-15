using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CoreLib
{
    public sealed class OnHover : CallbackBase, IPointerEnterHandler, IPointerExitHandler
    {
        public bool             _invert;
        public UnityEvent<bool> _onHover;

        // =======================================================================
        public void OnPointerEnter(PointerEventData eventData)
        {
            _onHover.Invoke(!_invert);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onHover.Invoke(_invert);
        }
    }
}