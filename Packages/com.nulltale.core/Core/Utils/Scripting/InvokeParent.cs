using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class InvokeParent : MonoBehaviour
    {
        public void Invoke()
        {
            var inv = GetComponentInParent<IInvokable>();
            if (inv != null)
                inv.Invoke();
        }
    }
}