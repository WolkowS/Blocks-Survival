using CoreLib.Values;
using UnityEngine;

namespace CoreLib
{
    public class LookAt : MonoBehaviour
    {
        public Vers<GameObject> _go;
        public float            _radius;

        private void LateUpdate()
        {
            transform.localPosition = transform.position.DirTo(_go.Value.transform.position).normalized * _radius;
        }
    }
}