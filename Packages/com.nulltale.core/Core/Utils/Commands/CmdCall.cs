using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdCall : CmdInvokeGo
    {
        // =======================================================================
        public override void Invoke(GameObject args)
        {
            Invoke(args.GetComponent<EventContainer>());
        }
        
        public void Invoke(EventContainer obj)
        {
            obj.Invoke();
        }
    }
}