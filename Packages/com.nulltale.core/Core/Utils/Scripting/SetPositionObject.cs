using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetPositionObject : MonoBehaviour
    {
        public Vers<GameObject>  _target;
        public Optional<Vector2> _random;
        public bool              _onEnable;

        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }
        
        public void Invoke()
        {
            var pos = _target.Value.transform.position;
            if (_random.Enabled)
            {
                var rand = _random.Value;
                pos += new Vector3(rand.x.Amplitude(), rand.y.Amplitude());
            }
            
            transform.position = pos;
        }        
    }
}