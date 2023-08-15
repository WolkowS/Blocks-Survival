using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    public class FromGlobalList : MonoBehaviour
    {
        public GlobalListGo _list;
        
        public Pick _pick;
        
        public Optional<Vector2> _range;

        public UnityEvent<GameObject> _onInvoke;
        
        // =======================================================================
        public enum Pick
        {
            Random,
            Nearest,
            Farthest,
        }
        
        // =======================================================================
        public void Invoke()
        {
            var content = _list.Value;
            
            if (_range.Enabled)
                content = content
                          .Where(n => _range.Value.Cointains(Vector3.Distance(transform.position, n.transform.position)))
                          .ToList();

            var pick = _pick switch
            {
                Pick.Random   => content.RandomOrDefault(),
                Pick.Nearest  => content.OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).FirstOrDefault(),
                Pick.Farthest => content.OrderByDescending(n => Vector3.Distance(transform.position, n.transform.position)).FirstOrDefault(),
                _             => throw new ArgumentOutOfRangeException()
            };
            
            if (pick != null)
                _onInvoke.Invoke(pick);
        }
    }
}