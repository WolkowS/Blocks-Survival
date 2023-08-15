using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerScreenSpace : Deployer
    {
        public float _padding;
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            pos = Core.Camera.
                       OrthographicBounds().
                       ToRectXY().
                       Inflate(_padding).
                       RandomPoint();
            
            rot = Quaternion.identity;
        }
    }
}