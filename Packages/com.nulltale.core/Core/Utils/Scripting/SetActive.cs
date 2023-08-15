using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetActive : MonoBehaviour
    {
        public bool                         m_Invert;
        public Optional<Vers<GameObject>> _go;
        
        public void Invoke(bool val)
        {
            var go = _go.Enabled ? _go.Value.Value : gameObject;
            go.SetActive(m_Invert ? !val : val);
        }
    }
}