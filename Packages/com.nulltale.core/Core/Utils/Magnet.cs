using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;
using CoreLib;

namespace CoreLib
{
    public class Magnet : MonoBehaviour
    {
        public Vers<GameObject> _target;

        public float _lerp = 1;
        public float _move = 1;
        
        public bool _disableOnReach = true;

        public Optional<UnityEvent<GameObject>> _onReached;
        
        // =======================================================================
        public void Assign(GvGo go)
        {
            _target.Value = go.Value;
        }
        
        public void Invoke()
        {
            _target.Value.transform.position = transform.position;
        }
        
        private void Update()
        {
            _target.Value.transform.position = Vector3.MoveTowards(Vector3.Lerp(_target.Value.transform.position, transform.position, _lerp * Time.deltaTime), transform.position, _move * Time.deltaTime);
           
            if (_onReached.Enabled && Vector3.Distance(_target.Value.transform.position, transform.position) <= 0.01f)
            {
                _onReached.Value.Invoke(_target.Value);
                if (_disableOnReach)
                    enabled = false;
            }
        }
    }
}