using CoreLib.Module;
using UnityEngine;

namespace CoreLib
{
    public class PointerScreen : MonoBehaviour
    {
        private void Update()
        {
            transform.position = Pointer.Screen;
        }
    }
}