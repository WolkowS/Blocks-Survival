using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using Pointer = CoreLib.Module.Pointer;

namespace CoreLib
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public static GameObject Current { get; set; }

        public  bool                 _anchored;
        public  Optional<GameObject> _root;
        public  UnityEvent           _on;
        public  UnityEvent           _off;
        private Vector3              _anchor;
        
        private GameObject Root => _root.GetValueOrDefault(gameObject);

        // =======================================================================
        public void OnBeginDrag(PointerEventData eventData)
        {
            Current = gameObject;
            _anchor = _anchored ? Root.transform.position.DirTo(Pointer.World) : Vector3.zero;
            
            _on.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Current = null;
            
            _off.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Root.transform.position = Pointer.World + _anchor;
        }
		
        public static void StopDrag()
        {
            StopDrag(Current);
        }

        public static void StopDrag(GameObject go)
        {
            if (go == null) 
                return;
            
            if (go != Current) 
                return;
            
            ExecuteEvents.Execute(go, new PointerEventData(EventSystem.current), ExecuteEvents.endDragHandler);
            typeof(InputSystemUIInputModule)
                .GetMethod("ResetPointers", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(EventSystem.current.currentInputModule, null);
        }
    }
}