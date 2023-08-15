using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public interface ITagContainer
    {
        internal IEnumerable<string> GetTags(); 
    }
    
    public class TagAsset : ScriptableObject
    {
        [Tag]
        public string _tag;
        
        // =======================================================================
        public bool CompareTag(Component comp)
        {
            if (comp is ITagContainer tc)
                return tc.GetTags().Any(n => _tag == n);
            
            return comp.CompareTag(_tag);
        }

        public bool CompareTag(GameObject go) => go.CompareTag(_tag);
    }
}