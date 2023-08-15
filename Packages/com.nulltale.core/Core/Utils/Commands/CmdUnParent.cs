using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdUnParent : CmdInvokeGo
    {
        public override void Invoke(GameObject args)
        {
            args.transform.SetParent(null);
        }
    }
}