using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate]
    public class SoFilter : ScriptableObject
    {
        public Optional<TagAsset> _tag;
        [ShowIf(nameof(_tag))]
        public bool               _tagTrue = true;
        
        // =======================================================================
        public virtual bool Check(GameObject go)
        {
            if (_tag.Enabled && go.CompareTag(_tag.Value._tag) != _tagTrue)
                return false;
            
            return true;
        }
    }
}