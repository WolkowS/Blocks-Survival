using System.Threading.Tasks;
using CoreLib;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class ScreenRollAsset : ScriptableObject
    {
        public ParticleSystem.MinMaxCurve _angleDeg;
        public bool                       _randomDir;
        public float                      _duration;
        [CurveRange]
        public AnimationCurve _curve;
        
        // =======================================================================
        [Button]
        public async void Invoke()
        {
            var angleDeg = _angleDeg.Evaluate();
            if (_randomDir)
                angleDeg *= UnityRandom.Bool() ? 1 : -1;
            
            var impact   = SimpleFx.Instance.AddRollImpact(angleDeg);
            var time     = 0f; 
            while (time < _duration)
            {
                impact._angle = _curve.Evaluate(time / _duration) * angleDeg;
                await Task.Yield();
                time += Time.deltaTime;
            }
            
            impact.Dispose();
        }
    }
}