using System;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Midi
{
    [ExecuteAlways]
    public class MidiReactNote : MidiOutput.MidiReact, IRefGet<float>
    {
        public MidiOutput.MidiNoteFilter _note;
        
        [HideInInspector]
        public Signal<float> _value;
        
        private MidiOutput _control;

        public float Value => _value.Value;
        
        // =======================================================================
        private void Awake()
        {
            _control = GetComponentInParent<MidiOutput>();            
        }

        private void OnEnable()
        {
            _control._reacts.Add(this);
        }

        private void OnDisable()
        {
            _control._reacts.Remove(this);
        }

        public override void NoteOn(int note, float vel)
        {
            if (_note.Check(note) == false)
                return;
            
            _value.Value = vel;
        }

        public override void NoteOff(int note)
        {
            if (_note.Check(note) == false)
                return;
            
            _value.Value = 0f;
        }
    }
}