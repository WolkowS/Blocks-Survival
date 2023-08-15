using CoreLib.Module;
using UnityEngine;

namespace CoreLib
{
    public class PointerWorld : MonoBehaviour
    {
        private void Update()
        {
            transform.position = Pointer.World;
        }
    }
}