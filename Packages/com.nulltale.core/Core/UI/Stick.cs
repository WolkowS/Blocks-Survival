using System;
using CoreLib;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreLib
{
    public class Stick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool             _interactable = true;
        public Vers<Vector3>    _screenPos;
        [RangeVec2(-1, 1, false)]
        public Vector2          _value;
        public float            _distance;
        public Image            _point;
        public Vector2          _output = new Vector2(0, 1);
        public AnimationCurve01 _lerp;

        public Color _normal;
        public Color _pressed;
        public Color _disabled;
        
        private bool       _isDrag;
        public UnityEvent<Vector2> _onChanged;
        public Vector2             Value
        {
            get => _value * Mathf.Lerp(_output.x, _output.y, _lerp.Evaluate(_value.magnitude));
            set
            {
                if (value.magnitude > 1f)
                    value.Normalize();
                
                _value = value;
                _point.transform.localPosition = _value * _distance;
                _onChanged.Invoke(Value);
            }
        }

        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                OnValidate();
            }
        }

        // =======================================================================
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var toVec = transform.position.DirTo(_screenPos);
            _validate(toVec);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDrag = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDrag)
                return;
            
            _validate(Vector3.zero);
        }

        private void OnValidate()
        {
            enabled = _interactable;
            
            _point.transform.localPosition = _value * _distance;
            _point.color                   = _interactable == false ? _disabled : _normal;
        }
        
        private void _validate(Vector3 toVec)
        {
            var norm = toVec.normalized;
            var dist = Extensions.Min(_distance, toVec.magnitude);
            _point.transform.localPosition = norm * dist;
            _value = norm * (dist / _distance);
            _onChanged.Invoke(Value);
        }

        public void OnPointerDown(PointerEventData eventData)
        {   
            _point.color = _pressed;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _point.color = _normal;
        }
    }
}