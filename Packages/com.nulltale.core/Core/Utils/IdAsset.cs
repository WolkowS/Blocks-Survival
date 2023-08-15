using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib
{
    public class IdAsset : ScriptableObject, IId
    {
        [SerializeField]
        private UniqueId m_Id;
        public string Key => m_Id;
    }
}