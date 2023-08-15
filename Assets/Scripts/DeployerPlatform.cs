using System.Linq;
using CoreLib;
using CoreLib.Spawn;
using UnityEngine;

namespace Game
{
    public class DeployerPlatform : Deployer
    {
        // =======================================================================
        public override void Locate(out Vector3 pos, out Quaternion rot)
        {
            var platform = GamePrefs.Instance._platforms.OfType<Platform>().Randomize().OrderBy(n => n._spawnWeight).First();
            pos = platform._spawn.transform.position + .1f.Amplitude().ToVector2X().To3DXY();
            rot = Quaternion.identity;;
        }
    }
}