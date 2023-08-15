using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Scripting
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class Pool : MonoBehaviour
    {
        public  List<Object> _content;
        private List<Object> _copy;
        
        // =======================================================================
        public T TakeRandom<T>() where T : class
        {
            var item = _content.RandomOrDefault();
            _content.Remove(item);
            return item as T;
        }
    }
}