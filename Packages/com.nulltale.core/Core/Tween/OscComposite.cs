using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscComposite : OscillatorBase
    {
        public List<OscillatorBase> _inputs;

        public override float Value
        {
            get
            {
                var result = 0f;
                foreach (var osc in _inputs)
                    result += osc.Value;
                
                return result;
            }
        }
        public override Vector2 Value2
        {
            get
            {
                var result = new Vector2();
                foreach (var osc in _inputs)
                    result += osc.Value2;
                
                return result;
            }
        }

        public override Vector3 Value3
        {
            get
            {
                var result = new Vector3();
                foreach (var osc in _inputs)
                    result += osc.Value3;
                
                return result;
            }
        }
    }
}