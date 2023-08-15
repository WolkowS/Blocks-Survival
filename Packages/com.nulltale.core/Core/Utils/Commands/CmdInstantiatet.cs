using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdInstantiatet : CmdInvokeGo
    {
        //public Optional<ParticleSystem.MinMaxCurve> _offset;
        public Optional<bool> _setActive;
        
        // =======================================================================
        public override void Invoke(GameObject args)
        {
            var result = Instantiate(args);
            
            if (_setActive.Enabled)
                result.SetActive(_setActive.Value);
        }
    }
}