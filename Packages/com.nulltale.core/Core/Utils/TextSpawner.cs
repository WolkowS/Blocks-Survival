using CoreLib.Module;
using UnityEngine;

namespace CoreLib
{
    public class TextSpawner : MonoBehaviour
    {
        public Optional<string> _format;
        public Optional<Color>  _color;
        
        // =======================================================================
        public void Invoke(int scores)
        {
            var text =_format.Enabled ? string.Format(_format.Value, scores) : scores.ToString();
            
            FxTools.SpawnText(text, transform.position, _color.GetValueOrDefault(default));
        }
    }
}