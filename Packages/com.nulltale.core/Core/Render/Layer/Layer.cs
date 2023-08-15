using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    public class Layer : ScriptableObject
    {
        public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;
        [NonSerialized] 
        public List<Renderer>  _list  = new List<Renderer>();
        public Optional<Color> _clear;
        [HideIf(nameof(_scale))]
        public bool            _depth = true;
        public Optional<float> _scale = new Optional<float>(1f);
    }
}