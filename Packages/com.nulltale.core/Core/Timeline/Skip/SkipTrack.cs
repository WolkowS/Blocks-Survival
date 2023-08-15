using System;
using CoreLib.Events;
using CoreLib.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.18f, 0.60f, 0.48f)]
    [TrackClipType(typeof(SkipAsset))]
    public class SkipTrack : TrackAsset
    {
        public Optional<GeVoid> _event;
        //public Optional<InputAction>  _inputContaner;
        public Optional<InputActionReference> _input;
        
        public bool _wait;
        
        private Action _onInput;
        
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var clips = GetClips();
            foreach(var clip in clips)
            {
                var clipAsset = (SkipAsset)clip.asset;
                clipAsset.m_Clip = clip;
                clipAsset.m_Track = this;
            }
 
            return base.CreateTrackMixer(graph, go, inputCount);
        }
        
        public void Subscribe(Action act)
        {
            var init = _onInput == null;
            
            _onInput += act;
            if (init)
            {
                if (_event.Enabled)
                    _event.Value.OnInvoke += _onInput;

                if (_input.Enabled)
                    _input.Value.action.performed += _inputAction;
            }
        }
        
        public void UnSubscribe(Action act)
        {
            _onInput -= act;
            var release = _onInput == null;
            if (release)
            {
                if (_event.Enabled)
                    _event.Value.OnInvoke -= _onInput;

                if (_input.Enabled)
                    _input.Value.action.performed -= _inputAction;
            }
        }
        
        private void _inputAction(InputAction.CallbackContext ctx)
        {
            _onInput.Invoke();
        }
    }

}