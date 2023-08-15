using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate(true)]
    public class LayersAsset : ScriptableObject
    {
        public SoCollection<LayerAsset> _collection;
    }
}