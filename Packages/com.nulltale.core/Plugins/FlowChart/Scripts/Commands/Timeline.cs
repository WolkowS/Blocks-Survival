using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Playables;

namespace Fungus
{
    [CommandInfo("Core", 
                 "Timeline", 
                 "Play timeline")]
    [AddComponentMenu("")]
    public class Timeline : Command
    {
        public Vers<PlayableDirector> _director;
        
        // =======================================================================
        public override void Execute()
        {
            _director.Value.Play();
            _director.Value.extrapolationMode =  DirectorWrapMode.None;
            _director.Value.stopped           += _continue;
        }

        public override void OnExit()
        {
            _director.Value.Stop();
        }

        private void OnDisable()
        {
            _director.Value.stopped -= _continue;
        }

        private void _continue(PlayableDirector pd)
        {
            _director.Value.stopped -= _continue;
            Continue();
        } 
        
        public override string GetSummary()
        {
            if (_director.Value == null)
                return "DIRECTOR IS NOT SET";
            
            return _director.Value.name;
        }

        public override Color GetButtonColor()
        {
            if (_director.Value == null)
                return Color.red;
            
            return new Color32(234, 235, 168, 255);
        }
    }
}