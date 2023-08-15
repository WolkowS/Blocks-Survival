using System;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayColor : PlayBase
    {
        public  Gradient     _color;
        private ColorAdapter _adapter;
        
        // =======================================================================
        private void Awake()
        {
            _adapter = new ColorAdapter(gameObject);
        }

        protected override void _onPlay(float scale)
        {
            _adapter.Color = _color.Evaluate(scale);
        }
    }
}