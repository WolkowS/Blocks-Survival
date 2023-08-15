using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Commands
{
    public class CmdSetColor : CmdInvokeGo
    {
        public ParticleSystem.MinMaxGradient _color;
        
        // =======================================================================
        public override void Invoke(GameObject args)
        {
            if (args.TryGetComponent<SpriteRenderer>(out var sr))
                sr.color = _color.Evaluate();
            else
            if (args.TryGetComponent<Image>(out var img))
                img.color = _color.Evaluate();
        }
    }
}