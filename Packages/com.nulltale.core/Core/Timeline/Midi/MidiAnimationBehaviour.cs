using System;
using CoreLib.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Midi
{
    // Runtime playable class that calculates MIDI based animation
    [Serializable]
    public sealed class MidiAnimationBehaviour : PlayableBehaviour
    {
        public  float       tempo = 120;
        public  uint        duration;
        public  uint        ticksPerQuarterNote = 96;
        public  MidiEvent[] events;
        private float       _previousTime;
        private MidiOutput  _output;

        public float DurationInSecond => duration / tempo * 60 / ticksPerQuarterNote;

        // =======================================================================
        public override void OnGraphStart(Playable playable)
        {
            _previousTime = (float)playable.GetTime();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // When the playable is being finished, signals laying in the rest
            // of the clip should be all triggered.
            if (!playable.IsDone()) 
                return;
            
            _triggerSignals(playable, _output, _previousTime, (float)playable.GetDuration());
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var current = (float)playable.GetTime();
            _output     = (MidiOutput)playerData;

            // Playback or scrubbing?
            if (info.evaluationType == FrameData.EvaluationType.Playback)
            {
                // Trigger signals between the prrevious/current time.
                _triggerSignals(playable, _output, _previousTime, current);
            }
            else
            {
                // Maximum allowable time difference for scrubbing
                const float maxDiff = 0.1f;

                // If the time is increasing and the difference is smaller
                // than maxDiff, it's being scrubbed.
                if (current - _previousTime < maxDiff)
                {
                    // Trigger the signals as usual.
                    _triggerSignals(playable, _output, _previousTime, current);
                }
                else
                {
                    // It's jumping not scrubbed, so trigger signals laying
                    // around the current frame.
                    var t0 = Mathf.Max(0, current - maxDiff);
                    _triggerSignals(playable, _output, t0, current);
                }
            }

            _previousTime = current;
        }
        
        // =======================================================================
        private void _triggerSignals(Playable playable, MidiOutput output, float previous, float current)
        {
            var t0 = _convertSecondToTicks(previous);
            var t1 = _convertSecondToTicks(current);

            // Resolve wrapping-around cases by offsetting.
            if (t1 < t0) t1 += (t0 / duration + 1) * duration;

            // Offset both the points to make t0 < duration.
            var offs = (t0 / duration) * duration;
            t0 -= offs;
            t1 -= offs;

            // Resolve loops.
            for (; t1 >= duration; t1 -= duration)
            {
                // Trigger signals between t0 and the end of the clip.
                _triggerSignalsTick(playable, output, t0, 0xffffffffu);
                t0 = 0;
            }

            // Trigger signals between t0 and t1.
            _triggerSignalsTick(playable, output, t0, t1);
        }

        private void _triggerSignalsTick(Playable playable, MidiOutput output, uint previous, uint current)
        {
            foreach (var e in events)
            {
                if (e.time >= current)
                    break;
                
                if (e.time < previous)
                    continue;
                
                if (e.IsNote == false)
                    continue;
                
                if (e.IsNoteOn)
                    output.NoteOn(e.Id, e.Velocity);
                else
                if (e.IsNoteOff)
                    output.NoteOff(e.Id);
            }
        }
        
        private uint _convertSecondToTicks(float time)
        {
            return (uint)(time * tempo / 60 * ticksPerQuarterNote);
        }

        private float _convertTicksToSecond(uint tick)
        {
            return tick * 60 / (tempo * ticksPerQuarterNote);
        }
    }
}
