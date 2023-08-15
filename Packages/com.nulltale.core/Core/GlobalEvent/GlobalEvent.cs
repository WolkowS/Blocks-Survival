using System;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib.Events
{
    [SoCreate(true)]
    public abstract class GlobalEvent : ScriptableObject
    {
        [ResizableTextArea]
        public string   _note;
    }
    
    public class GlobalEvent<T> : GlobalEvent
    {
        public Action<T> OnInvoke;
        
        public void Invoke(T agent) => OnInvoke?.Invoke(agent);
    }
}