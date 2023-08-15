using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Scriptiong
{
    public class DirectorSpeed : MonoBehaviour
    {
        public float _speed;
        public Optional<Vector2> _clamp;
        public bool  _onEnable = true;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke(_speed);
        }
     
        public void Invoke()
        {
            Invoke(_speed);
        }
        
        public void Invoke(float speed)
        {
            if (_clamp.Enabled)
                speed = speed.Clamp(_clamp.Value.x, _clamp.Value.y);
            
            GetComponent<PlayableDirector>().SetSpeed(speed);
        }
    }
}