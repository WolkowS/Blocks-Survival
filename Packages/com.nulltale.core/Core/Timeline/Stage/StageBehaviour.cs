using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class StageBehaviour : PlayableBehaviour
    {
        public StageAsset        _asset;
        public  StageAsset.Block _block;
        private bool             _played;
        public  PlayableDirector _director;
        public  TimelineClip     _clip;

        private int _index;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (_played)
                return;
                
            _index  = -100;
            _played = true;
            _block.OnEnter();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (_played == false)
                return;
            

            var scale = (float)((_director.time - _clip.start) / _clip.duration);
            _block.OnScale(scale);
            
            if (_asset._mode != StageAsset.Mode.None)
            {
                var index = _getIndex();
                if (_index != index)
                {
                    _index = index;
                    _block.OnBlock(_index);
                }
            }

            // -----------------------------------------------------------------------
            int _getIndex()
            {
                if (_asset._blocks.Length == 0)
                    return -1;
                
                switch (_asset._mode)
                {
                    case StageAsset.Mode.Relative:
                    {
                        var current = _asset._offset;
                        var time    = (float)(_director.time - _clip.start);
                        if (time < current)
                            return -1;
                        
                        for (var n = 0; n < _asset._blocks.Length; n++)
                        {
                            var step = _asset._blocks[n];
                            current += step;
                            if (time <= current)
                                return n;
                        }
                        
                        return _asset._blocks.Length - 1;
                    }
                    case StageAsset.Mode.Absolute:
                    {
                        var time    = (float)(_director.time - _clip.start);
                        if (time < _asset._blocks[0])
                            return -1;
                        
                        for (var n = 0; n < _asset._blocks.Length - 1; n++)
                        {
                            if (time >= _asset._blocks[n] && time <= _asset._blocks[n + 1])
                                return n;
                        }
                        
                        return _asset._blocks.Length - 1;   
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_played == false)
                return;
#if UNITY_EDITOR
            if (_director == null)
                return;
#endif
            if (_asset._mode != StageAsset.Mode.None)
            {
                if (_asset._blocks.Length > 0)
                {
                    // adjust last or first block index if playable out...
                    var index = _director.time > _clip.end ? _asset._blocks.Length - 1 : _director.time < _clip.start ? 0 : _index;
                    if (_index != index)
                    {
                        _index = index;
                        _block.OnBlock(_index);
                    }
                }
            }
            
            _played = false;
            _block.OnScale((float)((_director.time - _clip.start) / _clip.duration).Clamp01());
            _block.OnExit();
        }
    }
}