using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.FxTexture;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class Resolver : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _root;
        
        private Dictionary<string, GameObject>  _cahce = new Dictionary<string, GameObject>();

        // =======================================================================
        public IRef<T> GetRef<T>(string path)
        {
            if (_cahce.TryGetValue(path, out var go) == false)
            {
                go = (_root.Enabled ? _root.Value.Value : gameObject)
                     .transform
                     .Find(path)
                     .gameObject;
            }
            
            return go.GetComponent<IRef<T>>();
        }
    }
}