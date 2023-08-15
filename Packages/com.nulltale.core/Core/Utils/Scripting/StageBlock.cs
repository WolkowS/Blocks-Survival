using CoreLib.Timeline;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    public class StageBlock : StageAsset.Block
    {
        public Signal<float> _progress;
        public Signal<int>   _index;
        public SignalVoid    _onEnter;
        public SignalVoid    _onExit;

        private bool       _firstEnter;
        
        // =======================================================================
        public override void OnEnter()
        {
            _firstEnter = true;
            _onEnter.Notify();
        }

        public override void OnExit()
        {
            _onExit.Notify();
        }

        public override void OnScale(float scale)
        {
            _progress.Value = scale;
        }

        public override void OnBlock(int index)
        {
            if (_firstEnter )
            { 
                _firstEnter = false;
                if (_index.Value == index)
                    _index.Notify();
            }
            
            _index.Value = index;
        }

        public override void Init(TimelineClip clip, PlayableDirector director, Vector2 bounds)
        {
        }
    }
}