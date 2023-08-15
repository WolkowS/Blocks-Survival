using UnityEngine;

namespace Fungus
{
    [EventHandlerInfo("",
                      "On Invoke",
                      "The block will be executed when flowchart invoke method was called.")]
    [AddComponentMenu("")]
    public class OnInvokeEvent : EventHandler
    {
        //[SerializeField] protected int _waitForFrames = 1;

        internal void Invoke()
        {
            ExecuteBlock();
        }
    }
}