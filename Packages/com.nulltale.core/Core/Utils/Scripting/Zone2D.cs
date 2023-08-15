using System.Collections.Generic;
using CoreLib;
using UnityEngine;

namespace CoreLib.Scripting
{
    public abstract class Zone2D<T> : MonoBehaviour, IZone<T>
    {
        private HashSet<T>             m_Content = new HashSet<T>();
        public  IReadOnlyCollection<T> Content => m_Content;

        // =======================================================================
        protected virtual T _extract(Collider2D other) => other.attachedRigidbody != null ? other.attachedRigidbody.GetComponent<T>() : other.GetComponent<T>();

        protected virtual void _onEnter(T obj) { }
        protected virtual void _onExit(T obj) { }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            var content = _extract(other);
            if (!content.IsNull())
                if (m_Content.Add(content))
                    _onEnter(content);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var content = _extract(other);
            if (!content.IsNull())
                if (m_Content.Remove(content))
                    _onExit(content);
        }

        private void OnDisable()
        {
            m_Content.Clear();
        }
    }
}