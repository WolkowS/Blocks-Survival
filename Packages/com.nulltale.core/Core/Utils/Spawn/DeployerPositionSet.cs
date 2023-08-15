using System.Collections.Generic;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerPositionSet : Deployer
    {
        public List<Vers<GameObject>> _set;
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            var item = _set.Random();
            pos = item.Value.transform.position;
            rot = item.Value.transform.rotation;
        }
    }
}