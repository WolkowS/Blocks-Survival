using System;
using System.Linq;
using CoreLib.Events;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class DisableSiblings : MonoBehaviour
    {
        public void Invoke()
        {
            foreach (var trans in transform.parent.GetChildren().Where(n => n != transform))
                trans.gameObject.SetActive(false);
        }
    }
}