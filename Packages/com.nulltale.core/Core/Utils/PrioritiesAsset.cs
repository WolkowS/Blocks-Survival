using CoreLib;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate(true)]
    public class PrioritiesAsset : ScriptableObject
    {
        public SoCollection<PriorityAsset> _list;
    }
}