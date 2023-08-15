using System;
using System.Collections;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace CoreLib.SpriteAnimation
{
    [RequireComponent(typeof(Animator))]
    public class OneShotAnimation : MonoBehaviour
    {
        public AnimationClip _clip;
        public float         _speed = 1f;
        
        // =======================================================================
        [Button]
        public async void Play()
        {
            var _graph    = PlayableGraph.Create();
            var _animator = GetComponent<Animator>();
            
            var clipPlayable   = AnimationClipPlayable.Create(_graph, _clip);
            var playableOutput = AnimationPlayableOutput.Create(_graph, "Out", _animator);
            
            clipPlayable.SetSpeed(_speed);

            // Connect the Playable to an output
            playableOutput.SetSourcePlayable(clipPlayable);
            _graph.Play();
            
            var endTime = Time.time + _clip.length / _speed;
            while (endTime >= Time.time)
                await Task.Yield();
            
            if (_graph.IsValid())
                _graph.Destroy();
        }
    }
}