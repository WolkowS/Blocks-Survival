using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class MatProp
    {
        public string Name;
        public int    Hash;
        
        // =======================================================================
        public MatProp()
        {
        }
        
        public MatProp(string name)
        {
            this.Name = name;
            Hash = Shader.PropertyToID(name);
        }
        
        public static implicit operator int(MatProp mp) => mp.Hash;
    }
}