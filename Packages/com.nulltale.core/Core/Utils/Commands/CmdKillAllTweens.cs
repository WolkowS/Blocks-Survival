using DG.Tweening;
using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdKillAllTweens : CmdInvoke
    {
        public bool _complete;
        
        // =======================================================================
        public override void Invoke()
        {
            DOTween.KillAll(_complete);
        }
    }
}