using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class BroadcastMessage : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _root;
        public string                     _message;
        
        // =======================================================================
        public void Invoke()
        {
            foreach (var child in (_root.Enabled ? _root.Value.Value : gameObject).GetChildren())
            {
                child.BroadcastMessage(_message);
            }
        }
    }
}