using System;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    [EventHandlerInfo("",
                      "On Enable",
                      "The block will execute when the game object enabled.")]
    [AddComponentMenu("")]
    public class OnEnableEvent : EventHandler
    {
        //[SerializeField] protected int _waitForFrames = 1;

        private void OnEnable()
        {
            ExecuteBlock();
        }
    }
}