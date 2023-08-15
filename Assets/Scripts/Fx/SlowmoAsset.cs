using CoreLib.Module;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class SlowmoAsset : ScriptableObject
    {
        [Range(0, 1)]
        public float _timeScale;
        public float _duration;

        // =======================================================================
        [Button]
        public void Invoke()
        {
            TimeControl.SlowDown(_timeScale, _duration);
        }
    }
}