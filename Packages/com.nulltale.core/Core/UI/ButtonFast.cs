using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreLib
{
    public class ButtonFast : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image _sourceImage;
        public Color _normal;
        public Color _hover;
        
        public UnityEvent _onDown;
        
        // =======================================================================
        public void OnPointerDown(PointerEventData eventData)
        {
            _onDown.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _sourceImage.color = _hover;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _sourceImage.color = _normal;
        }
    }
}