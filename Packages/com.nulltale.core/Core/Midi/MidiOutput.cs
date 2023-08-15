using System;
using System.Collections.Generic;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Midi
{
    public class MidiOutput : MonoBehaviour
    {
        [NonSerialized]
        public List<MidiReact> _reacts = new List<MidiReact>();
        
        // =======================================================================
        public abstract class MidiReact : MonoBehaviour
        {
            public abstract void NoteOn(int note, float vel);
            public abstract void NoteOff(int note);
        }
        
        [Serializable]
        public struct MidiNoteFilter
        {
            public MidiNote note;
            public MidiOctave octave;

            public bool Check(int key)
            {
                return (octave == MidiOctave.Any || key / 12 == (int)octave - 1) && 
                       (note   == MidiNote  .All || key % 12 == (int)note   - 1);
            }
        }
        
        [Serializable]
        public class Key : IRefGet<float>
        {
            public  Vers<float> _value;
            private bool        _pressed;
            private bool        _run;
            private float       _vel;
            private float       _time;
            private float       _scale;
            private Adsr        _adsr;

            public float Value => _value.Value;
            
            // =======================================================================
            public void On(float vel)
            {
                _pressed = true;
                _run = true;
                _vel = vel;
            }
            
            public void Off()
            {
                _pressed = false;
            }
            
            public void Init(Adsr adsr, float scale)
            {
                _adsr  = adsr;
                _scale = scale;
            }
            
            public void Update()
            {
                if (_run == false)
                    return;
                
                _value.Value = _adsr.Update(_pressed, ref _time, Time.deltaTime) * _vel * _scale;
                if (_adsr.IsComplete)
                    _run = false;
            }
        }
        
        public enum MidiNote
        {
            All,
            C,
            CSharp,
            D,
            DSharp,
            E,
            F,
            FSharp,
            G,
            GSharp,
            A,
            ASharp,
            B
        }

        public enum MidiOctave
        {
            Any,
            Minus1,
            Zero,
            Plus1,
            Plus2,
            Plus3,
            Plus4,
            Plus5,
            Plus6,
            Plus7,
            Plus8,
            Plus9
        }

        // =======================================================================
        public void NoteOn(int note, float vel)
        {
            foreach (var rect in _reacts)
                rect.NoteOn(note, vel);
        }
        
        public void NoteOff(int note)
        {
            foreach (var rect in _reacts)
                rect.NoteOff(note);
        }
    }
}