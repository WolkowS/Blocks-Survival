using System.Linq;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class DeployerScreenEdge : Deployer
    {
        public float _padding;
        
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            var camPos = Core.Camera.transform.position.To2DXY();
            var line   = new Line(camPos, camPos + UnityRandom.Normal2D(1000f));
            var hit    = Vector2.zero;
            
            var hasHit = Core.Camera
                              .OrthographicBounds()
                              .ToRectXY()
                              .Inflate(_padding)
                              .Lines()
                              .FirstOrDefault(n => n.Intersect(line, out hit));
            
            pos = hit.To3DXY();
            rot = Quaternion.identity;
        }
    }
}