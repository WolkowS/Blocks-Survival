using UnityEngine;

namespace CoreLib
{
    public class AudioAnalizerVisualizator : MonoBehaviour
    {
        public AudioAnalizer _analizer;
        
        // =======================================================================
        private void Update()
        {
            var _spectrum = _analizer.Data;
            var step      = 1f / _spectrum.Length;
            var pos       = -(step * _spectrum.Length * .5f);
            var world     = transform.position;
            
            for (var n = 1; n < _spectrum.Length - 1; n++)
            {
                Debug.DrawLine(new Vector3(pos, _spectrum[n], 0) + world, new Vector3(pos + step, _spectrum[n + 1], 0) + world, Color.red);
                pos += step;
            }
        }
    }
}