using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class GoProvider : MonoBehaviour, IRefGet<GameObject>
    {
        public Source _source;
        [Label("Target")]
        [HideIf(nameof(_source), Source.Direct)]
        public Mode _selector;
        
        [Label("Target")]
        [ShowIf(nameof(_source), Source.Direct)]
        public Vers<GameObject> _target;
        
        [Label("Target")]
        [ShowIf(nameof(_source), Source.GlobalList)]
        public GlobalListGo _targetsGlobalList;
        
        [Label("Target")]
        [ShowIf(nameof(_source), Source.List)]
        public List<GameObject> _targetsList;
        
        
        public GameObject Value
        {
            get
            {
                GameObject item = null;
                switch (_source)
                {
                    case Source.Direct:
                    {
                        item = _target.Value;
                    } break;
                    
                    case Source.GlobalList:
                    case Source.List:
                    {
                        var list = _source == Source.GlobalList ? _targetsGlobalList.Value : _targetsList;
                        item = _selector switch
                        {
                            Mode.Random   => list.RandomOrDefault(),
                            Mode.Nearest  => list.OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).FirstOrDefault(),
                            Mode.Farthest => list.OrderByDescending(n => Vector3.Distance(transform.position, n.transform.position)).FirstOrDefault(),
                            _             => throw new ArgumentOutOfRangeException()
                        };
                    } break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                } 

                return item;
            }
        }
        
        // =======================================================================
        public enum Mode
        {
            Random,
            Nearest,
            Farthest,
        }

        public enum Source
        {
            Direct,
            GlobalList,
            List
        }
    }
}