using CodeWriter.ExpressionParser;
using CoreLib.Values;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class FloatExpression : MonoBehaviour
    {
        [TextArea]
        public string _expression = "a >= b AND NOT(b) OR (a + b) >= c";
        
        public bool _onEnable;
        
        public SerializableDictionary<string, Vers<float>> _floats;
        
        public  UnityEvent<float> _onInvoke;
        private Expression<float> _compiled;

        // =======================================================================
        private void Awake()
        {
            // create context
            var context = new ExpressionContext<float>();

            // register some variables
            foreach (var kv in _floats)
                context.RegisterVariable(kv.Key, () => kv.Value.Value);

            // compile expression
            _compiled = FloatExpressionParser.Instance.Compile(_expression, context, true);
        }

        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                Awake();
#endif
            _onInvoke.Invoke(_compiled.Invoke());
        }
    }
}