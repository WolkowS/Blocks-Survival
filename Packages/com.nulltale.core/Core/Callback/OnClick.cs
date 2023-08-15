using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CoreLib
{
    public sealed class OnClick : CallbackBase, IPointerClickHandler
    {
        public UnityEvent Action;

        // =======================================================================
        public void OnPointerClick(PointerEventData eventData)
        {
            Action.Invoke();
        }
        
        public void Invoke()
        {
            Action.Invoke();
        }
    }
}