using System.Collections.Generic;
using NativeSerializableDictionary;
using UnityEngine;

namespace CoreLib
{
    public class RefLinkResolver : MonoBehaviour
    {
        public interface IResolvable
        {
            void Resolve(RefLinkResolver resolver);
        }
        
        public SerializableDictionary<ResolverId, Object> _links;
    }
}