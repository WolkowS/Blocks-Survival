using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    public class CombineColor : MonoBehaviour
    {
        public  Vers<GvColor> _output;
        private Color   _value;
        public float R
        {
            set { _value.r = value; enabled = true; }
        }

        public float G
        {
            set { _value.g = value; enabled = true;}
        }

        public float B
        {
            set { _value.b = value; enabled = true;}
        }

        public float A
        {
            set { _value.a = value; enabled = true;}
        }

        // =======================================================================
        private void Update()
        {
            _output.Value.Value = _value;
            enabled       = false;
        }
    }
}