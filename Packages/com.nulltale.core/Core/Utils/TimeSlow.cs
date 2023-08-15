using CoreLib.Module;
using UnityEngine;

namespace CoreLib
{
    public class TimeSlow : MonoBehaviour
    {
        [Range(0, 1)]
        public float _scale;
        public float _duration;
        
        // =======================================================================
        public void Invoke()
        {
            TimeControl.SlowDown(_scale, _duration);
        }
    }
}