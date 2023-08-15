using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class ScreenFrameAsset : ScriptableObject
    {
        public float    _duration;
        public float    _impact;
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            SimpleFx.Instance.AddOrthoImpact(_duration, _impact);
        }
    }
}