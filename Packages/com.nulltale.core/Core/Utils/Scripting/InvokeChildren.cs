using UnityEngine;

namespace CoreLib.Scripting
{
    public class InvokeChildren : MonoBehaviour
    {
        public void Invoke()
        {
            foreach (var child in GetComponentsInChildren<IInvokable>())
                child.Invoke();
        }
    }
}