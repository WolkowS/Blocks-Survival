using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetLayer : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _go;
        
        public LayerAsset _asset;
        
        // =======================================================================
        public void Invoke()
        {
            var go = _go.Enabled ? _go.Value : gameObject;
            go.layer = _asset._layer;
        }
    }
}