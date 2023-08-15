using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate(true)]
    public class TagsAsset : ScriptableObject
    {
        public SoCollection<TagAsset> _tags;
    }
}