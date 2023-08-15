using System;
using CoreLib.Scripting;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.Commands
{
    public class CmdDestroy : CmdInvokeGo
    {
        public Destroy.Method _method;
        [ShowIf(nameof(_method), Scripting.Destroy.Method.Default)]
        public Optional<float>  _delay;
        
        // =======================================================================
        public override void Invoke(GameObject args)
        {
            if (_method == Scripting.Destroy.Method.Default && _delay.Enabled && _delay.Value > 0f)
            {
                Destroy(args, _delay.Value);
                var handle = CreateHandle();
                Core.Instance.Delayed(() => handle.Complete(), _delay.Value);
            }
                    
            switch (_method)
            {
                case Scripting.Destroy.Method.Default:
                {
                    if (_delay.Enabled && _delay.Value > 0f)
                    {
                        Destroy(args, _delay.Value);
                    }
                    else
                    {
                        Destroy(args);
                    }
                } break;
                
                case Scripting.Destroy.Method.Immediate:
                {
                    DestroyImmediate(args);
                } break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

    