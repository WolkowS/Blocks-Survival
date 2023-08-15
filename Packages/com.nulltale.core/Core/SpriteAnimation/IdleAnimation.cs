using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace CoreLib.SpriteAnimation
{
    [RequireComponent(typeof(Animator))]
    public class IdleAnimation : MonoBehaviour
    {
        public  AnimationClip _clip;
        public  float         _speed = 1f;
        private PlayableGraph _graph;

        // =======================================================================
        public void Start()
        {
            _graph = PlayableGraph.Create();
            var _animator = GetComponent<Animator>();
            
            var clipPlayable   = AnimationClipPlayable.Create(_graph, _clip);
            var playableOutput = AnimationPlayableOutput.Create(_graph, "Out", _animator);
            
            clipPlayable.SetSpeed(_speed);
            clipPlayable.SetTime(Random.value * _clip.length);

            // Connect the Playable to an output
            playableOutput.SetSourcePlayable(clipPlayable);
            _graph.Play();
        }

        private void OnDestroy()
        {
            if (_graph.IsValid())
                _graph.Destroy();
        }
    }
}