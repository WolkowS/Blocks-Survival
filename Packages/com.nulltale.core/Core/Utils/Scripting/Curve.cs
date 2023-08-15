using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Curve : MonoBehaviour, IRefGet<AnimationCurve01>
    {
        public Vector2                _input  = new Vector2(0, 1);
        public Vector2                _output = new Vector2(0, 1);
        public Vers<AnimationCurve01> _curve;

        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke(float val)
        {
            val = val.Clamp(_input.x, _input.y) / _input.Segment();
            
            _onInvoke.Invoke(Mathf.Lerp(_output.x, _output.y, _curve.Value.Evaluate(val)));
        }

        public AnimationCurve01 Value => _curve;
    }
}