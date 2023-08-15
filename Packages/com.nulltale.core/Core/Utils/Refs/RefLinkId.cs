using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class RefLinkId<T> : RefLink<T>, RefLinkResolver.IResolvable
    {
        [SerializeField]
        protected ResolverId _id;
            
        // =======================================================================
        public void Resolve(RefLinkResolver resolver)
        {
            if (_resolved)
                return;
            
            _obj = resolver._links[_id];
            _resolve();
        }
    }
}