using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Commands
{
    public class CmdTouch : CmdInvokeGo
    {
        public Optional<ParticleSystem.MinMaxCurve> _move;
        [ShowIf(nameof(_move))]
        public ParticleSystem.MinMaxCurve           _moveTime;
        [ShowIf(nameof(_move))]
        public Optional<Vector3>                    _moveDir;
        [ShowIf(nameof(_move))]
        public Optional<AnimationCurve01>           _moveEase;
        
        public Optional<ParticleSystem.MinMaxCurve> _scale;
        [ShowIf(nameof(_scale))]
        public ParticleSystem.MinMaxCurve           _scaleTime;
        [ShowIf(nameof(_scale))]
        public Optional<Vector3>                    _scaleWeight;
        [ShowIf(nameof(_scale))]
        public Optional<AnimationCurve01>           _scaleEase;
        
        public Optional<ParticleSystem.MinMaxCurve> _rotate;
        [ShowIf(nameof(_rotate))]
        public ParticleSystem.MinMaxCurve           _rotateTime;
        [ShowIf(nameof(_rotate))]
        public Optional<Vector3>                    _rotateAxis;
        [ShowIf(nameof(_rotate))]
        public Optional<AnimationCurve01>           _rotateEase;

        // =======================================================================
        public override void Invoke(GameObject args)
        {
            var go = args;
            
            var totalTime = 0f;
            if (_move.Enabled)
            {
                var time = _moveTime.Evaluate();
                
                totalTime = time;
                
                var offset = _move.Value.Evaluate();
                var dir    = _moveDir.Enabled ? _moveDir.Value : UnityRandom.Normal2D().To3DXY();
                go.transform
                  .DOLocalMove(go.transform.localPosition + offset * dir, time)
                  .SetEase(_moveEase.Value._curve);
            }
            
            if (_scale.Enabled)
            {
                var time = _scaleTime.Evaluate();
                
                if (totalTime < time)
                    totalTime = time;
                
                var scale  = _scale.Value.Evaluate();
                var weight = _scaleWeight.Enabled ? _scaleWeight.Value : Vector3.one;
                
                go.transform
                  .DOScale(go.transform.localScale + scale * weight, time)
                  .SetEase(_scaleEase.Value._curve);
            }
            
            if (_rotate.Enabled)
            {
                var time = _rotateTime.Evaluate();
                
                if (totalTime < time)
                    totalTime = time;
                
                var angle = _rotate.Value.Evaluate();
                var axis  = _rotateAxis.Enabled ? _rotateAxis.Value : UnityRandom.Choose(Vector3.back, Vector3.forward);
                
                go.transform
                  .DORotateQuaternion(go.transform.rotation * Quaternion.AngleAxis(angle, axis), time)
                  .SetEase(_rotateEase.Value._curve);
            }
            
            var endTime = Time.time  + totalTime;
            CreateHandle(() => endTime > Time.time);
        }
    }
}