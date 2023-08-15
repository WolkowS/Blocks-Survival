using System;
using CoreLib;
using CoreLib.Values;
using UnityEngine;

namespace Game
{
    public class Sticky : MonoBehaviour
    {
        public  Rigidbody2D _root;
        public  GameObject  _ground;
        private Vector3     _posLast;
        
        // =======================================================================
        private void OnTriggerEnter2D(Collider2D col)
        {
            _ground  = col.attachedRigidbody.gameObject;
            _posLast = _ground.transform.position;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_ground != other.attachedRigidbody.gameObject)
                return;
            
            _ground = null;
        }

        private void OnDisable()
        {
            _ground = null;
        }

        private void LateUpdate()
        {
            if (_ground == null)
                return;
            
            var offset = _ground.transform.position - _posLast;
            _root.gameObject.transform.position += offset.WithY(0f);
            _posLast           =  _ground.transform.position;
        }
    }
}