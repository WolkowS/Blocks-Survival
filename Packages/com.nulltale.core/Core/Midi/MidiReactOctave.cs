using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Midi
{
    [ExecuteAlways]
    public class MidiReactOctave : MidiOutput.MidiReact
    {
        public MidiOutput.MidiOctave _octave;
        
        [HideInInspector] public Signal<float> _c;
        [HideInInspector] public Signal<float> _cSharp;
        [HideInInspector] public Signal<float> _d;
        [HideInInspector] public Signal<float> _dSharp;
        [HideInInspector] public Signal<float> _e;
        [HideInInspector] public Signal<float> _f;
        [HideInInspector] public Signal<float> _fSharp;
        [HideInInspector] public Signal<float> _g;
        [HideInInspector] public Signal<float> _gSharp;
        [HideInInspector] public Signal<float> _a;
        [HideInInspector] public Signal<float> _aSharp;
        [HideInInspector] public Signal<float> _b;
        
        private MidiOutput _control;
        
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
            if ((_octave == MidiOutput.MidiOctave.Any || note / 12 == (int)_octave - 1) == false)
                return;

            _getNote(note).Value = vel;
        }

        public override void NoteOff(int note)
        {
            if ((_octave == MidiOutput.MidiOctave.Any || note / 12 == (int)_octave - 1) == false)
                return;
            
            _getNote(note).Value = 0f;
        }
        
        public IEnumerable<Signal<float>> Notes()
        {
                yield return _c;
                yield return _cSharp;
                yield return _d;
                yield return _dSharp;
                yield return _e;
                yield return _f;
                yield return _fSharp;
                yield return _g;
                yield return _gSharp;
                yield return _a;
                yield return _aSharp;
                yield return _b;
        }
        
        // =======================================================================
        private Signal<float> _getNote(int note)
        {
            return (MidiOutput.MidiNote)(note % 12 + 1) switch
            {
                MidiOutput.MidiNote.C      => _c,
                MidiOutput.MidiNote.CSharp => _cSharp,
                MidiOutput.MidiNote.D      => _d,
                MidiOutput.MidiNote.DSharp => _dSharp,
                MidiOutput.MidiNote.E      => _e,
                MidiOutput.MidiNote.F      => _f,
                MidiOutput.MidiNote.FSharp => _fSharp,
                MidiOutput.MidiNote.G      => _g,
                MidiOutput.MidiNote.GSharp => _gSharp,
                MidiOutput.MidiNote.A      => _a,
                MidiOutput.MidiNote.ASharp => _aSharp,
                MidiOutput.MidiNote.B      => _b,
                MidiOutput.MidiNote.All    => throw new ArgumentOutOfRangeException(),
                _                          => throw new ArgumentOutOfRangeException()
            };
        }
    }
}