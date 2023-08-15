using CoreLib.Scripting;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerZone : Deployer
    {
        public Vers<Zone> _zone;
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            pos = _zone.Value.RandomPoint();
            rot = Quaternion.identity;
        }
    }
}