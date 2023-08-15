using CoreLib.Values;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate(true)]
    public class Filter : ScriptableObject
    {
        public Optional<GvGo>     _isEqual;
        public Optional<TagAsset> _hasTag;
        
        // =======================================================================
        public bool Check(GameObject go)
        {
            if (_isEqual.Enabled && _isEqual.Value.Value != go)
                return false;
            
            if (_hasTag.Enabled && _hasTag.Value.CompareTag(go) == false)
                return false;
            
            return true;
        }
    }
}