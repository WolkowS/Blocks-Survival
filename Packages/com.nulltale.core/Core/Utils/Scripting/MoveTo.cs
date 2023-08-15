using CoreLib.Values;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class MoveTo : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _root;
        public Vers<GameObject>           _goal;

        public float _duration;
        
        [CurveRange]
        public AnimationCurve _lerp;

        public UnityEvent _onComplete;
        
        // =======================================================================
        public void Invoke()
        {
            var root = (_root.Enabled ? _root.Value.Value : gameObject);
            LeanTween
                .move(root.gameObject, _goal.Value.transform.position, _duration)
                .setEase(_lerp)
                .setOnComplete(_onComplete.Invoke);
        }
    }
}