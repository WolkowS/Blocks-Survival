using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public class GradientBlender : MonoBehaviour
    {
        public Texture2D _output;
        [NonSerialized]
        public List<GradientBlenderTex> _grads = new List<GradientBlenderTex>();
        
        public int Width => _output.width;
        
        // =======================================================================
        private void Update()
        {
            var grads = _grads.Where(n => n.Weight > 0f).ToArray();
            var weight = grads.Sum(n => n.Weight);
            
            foreach (var grad in grads)
                grad.Require();
            
            for (var x = 0; x < Width; x++)
            {
                var colorSum = Color.clear;
                foreach (var grad in grads)
                    colorSum += grad._tex.GetPixel(x, 0) * grad.Weight;
                _output.SetPixel(x, 0, colorSum /= weight);
            }
            _output.Apply(false, false);
            
            enabled = false;
        }
    }
}