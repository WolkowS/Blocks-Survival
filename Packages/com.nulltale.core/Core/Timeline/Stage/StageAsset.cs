using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class StageAsset : PlayableAsset, ITimelineClipAsset
    {
        public ExposedReference<Block> _stage;
        public Mode                    _mode;
        private Mode                   _modePrev;
        [ShowIf(nameof(_mode), Mode.Relative)]
        public float                   _offset;
        [HideIf(nameof(_mode), Mode.None)]
        public float[]                 _blocks;
        [NonSerialized]
        public TimelineClip            _clip;

        [NonSerialized]
        public StageTrack _track;

        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public enum Mode
        {
            None,
            Relative,
            Absolute
        }

        // =======================================================================
        public abstract class Block : MonoBehaviour
        {
            public virtual void Init(TimelineClip clip, PlayableDirector director, Vector2 bounds)
            {
            }
            
            public virtual void OnEnter()
            {
            }
            
            public virtual void OnExit()
            {
            }
            
            public virtual void OnBlock(int index)
            {
            }

            public virtual void OnScale(float scale)
            {
            }
        }
        
        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<StageBehaviour>.Create(graph);
            
            var beh = playable.GetBehaviour();
            beh._block = _stage.Resolve(graph.GetResolver());
            
            if (beh._block == null)
                return Playable.Null;
            
            beh._director = go.GetComponent<PlayableDirector>();
            beh._asset    = this;
            beh._clip     = _clip;
            beh._block.Init(_clip, beh._director, new Vector2((float)_track._start.GetValueOrDefault(0), (float)_track._end.GetValueOrDefault(beh._director.duration)));

            return playable;
        }

        private void OnValidate()
        {
            if (_mode == Mode.None)
                return;
            
            if (_mode == _modePrev)
                return;

            _modePrev = _mode;
            switch (_mode)
            {
                case Mode.Relative:
                {
                    _offset = _blocks[0];
                    for (var n = 0; n < _blocks.Length - 1; n++)
                    {
                        _blocks[n] = _blocks[n + 1] - _blocks[n];
                    }
                    _blocks[^1] = 0; 
                    
                } break;
                case Mode.Absolute:
                {
                    var time = 0f;
                    for (var n = 0; n < _blocks.Length; n++)
                    {
                        var blockTime = _blocks[n];
                        _blocks[n] = time + _offset;
                        time += blockTime;
                    }
                } break;
                case Mode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}