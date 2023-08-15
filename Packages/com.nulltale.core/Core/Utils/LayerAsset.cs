using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class LayerAsset : ScriptableObject
    {
        [Layer]
        public int _layer;
        
        public bool CompareLayer(GameObject go) => go.layer == _layer;
    }
}