using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerPosition : Deployer
    {
        public Vers<GameObject> _position;
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            pos = _position.Value.transform.position;
            rot = _position.Value.transform.rotation;
        }
    }
}