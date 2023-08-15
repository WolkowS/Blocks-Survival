using UnityEngine;

namespace CoreLib.Spawn
{
    public abstract class Deployer : MonoBehaviour
    {
        public abstract void Locate(out Vector3 pos, out Quaternion rot);
    }
}