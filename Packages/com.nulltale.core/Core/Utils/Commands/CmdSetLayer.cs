using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdSetLayer : CmdInvokeGo
    {
        [Layer]
        public int _layer;
        
        // =======================================================================
        public override void Invoke(GameObject args)
        {
            args.layer = _layer;
        }
    }
}