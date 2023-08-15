using System;
using CoreLib.Timeline;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Play
{
    [ExecuteAlways]
    public class StagePlay : StageAsset.Block, IPlay
    {
        public Optional<AnimationCurve01> _lerp;
        
        public event Action<float> OnPlay;
        
        public bool _activation;
        [ShowIf(nameof(_activation))]
        public bool _activate = true;
        [ShowIf(nameof(_activation))]
        public bool _deactivate = true;
        
        // =======================================================================
        public override void Init(TimelineClip clip, PlayableDirector director, Vector2 bounds)
        {
            if (_activation && _deactivate)
                gameObject.SetActive(false);
        }

        public override void OnEnter()
        {
            if (_activation == false || _activate == false)
                return;
            
            gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            if (_activation == false || _deactivate == false)
                return;
            
            gameObject.SetActive(false);
        }

        public override void OnScale(float scale)
        {
            OnPlay?.Invoke(_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale);
        }
    }
    
    [ExecuteAlways]
    public abstract class PlayBase : MonoBehaviour
    {
        private IPlay   _play;
        public Optional<AnimationCurve01> _lerp;
        protected float _scalePrev = float.MinValue;

        // =======================================================================
        private void OnEnable()
        {
            _play = transform.GetComponentInParent<IPlay>();
            _play.OnPlay += _onPlayWrapper; 
        }

        private void OnDisable()
        {
            _play.OnPlay -= _onPlayWrapper;
        }

        // =======================================================================
        private void _onPlayWrapper(float scale)
        {
            if (scale == _scalePrev)
                return;
            
            _scalePrev = scale;
            _onPlay(_lerp.Enabled ? _lerp.Value.Evaluate(scale) : scale);
        }
        
        protected abstract void _onPlay(float scale);
    }
    
    [ExecuteAlways]
    public abstract class OnPlayBase : MonoBehaviour
    {
        private IPlay   _play;

        // =======================================================================
        private void OnEnable()
        {
            _play = transform.GetComponentInParent<IPlay>();
            _play.OnPlay += _onPlay; 
        }

        private void OnDisable()
        {
            _play.OnPlay -= _onPlay;
        }

        // =======================================================================
        protected abstract void _onPlay(float scale);
    }

    public interface IPlay
    {
        event Action<float> OnPlay;
    }
}