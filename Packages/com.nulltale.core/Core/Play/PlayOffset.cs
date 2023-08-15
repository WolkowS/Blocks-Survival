using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayOffset : PlayBase
    {
        public Transform            _target;
        public  Optional<Transform> _end;
        [HideIf(nameof(_end))]
        public  Vector3             _offset;
        private Vector3             _initial;
        
        // =======================================================================
        private void Awake()
        {
            _initial = transform.position;
        }

        protected override void _onPlay(float scale)
        {
            _target.position = Vector3.LerpUnclamped(_initial, _end.Enabled ? _end.Value.transform.position : _initial + _offset, scale);
        }
    }
}