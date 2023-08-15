using System;
using System.Collections.Generic;
using CoreLib;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class TriggerSensor : CallbackBase
    {
        public bool             _invert;
        public UnityEvent<bool> _hasContent;
        [NonSerialized]
        public List<Collider2D> _content = new List<Collider2D>();

        // =======================================================================
        private void OnTriggerEnter2D(Collider2D other)
        {
            _content.Add(other);
            
            if (_content.Count == 1)
                _hasContent.Invoke(_invert ? false : true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _content.Remove(other);
            
            if (_content.Count == 0)
                _hasContent.Invoke(_invert ? true : false);
        }
    }
}