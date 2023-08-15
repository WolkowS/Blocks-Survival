using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Play
{
    public class PlayNode : PlayNodeBase
    {
        public Optional<AnimationCurve01> _lerp;
        
        // =======================================================================
        protected override void _onPlay(float scale)
        {
            _invoke(_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale);
        }
    }
    
    [ExecuteAlways]
    public abstract class PlayNodeBase : MonoBehaviour, IPlay
    {
        private IPlay              _play;
        public event Action<float> OnPlay;
        protected float            _scalePrev = float.MinValue;
        
        // =======================================================================
        private void OnEnable()
        {
            _play = transform.parent.GetComponentInParent<IPlay>();
            _play.OnPlay += _onPlayWrapper; 
        }

        private void OnDisable()
        {
            _play.OnPlay -= _onPlayWrapper;
        }

        // =======================================================================
        protected void _invoke(float scale) => OnPlay?.Invoke(scale);
        protected void _onPlayWrapper(float scale)
        {
            if (_scalePrev == scale)
                return;
            
            _scalePrev = scale;
            
            _onPlay(scale);
        }
        protected abstract void _onPlay(float scale);
    }
}