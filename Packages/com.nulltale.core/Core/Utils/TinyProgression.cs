using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TinyProgression : ISerializationCallbackReceiver
    {
        [SerializeField]
        private float[]        _data;
        private List<Interval> _intervals;

        private Interval _current;
        [SerializeField]
        private float    _position;

        public float Excess => _position - _current.Start;
        public float Scale  => Excess / _current.Lenght;
        public int   Index  => _current.Index;
        public float Total
        {
            get
            { 
                var last = _intervals.Last();
                return last.Start + last.Lenght;
            }
        }

        public float Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;

                _position = value;

                _current = IntervalAt(_position);
            }
        }

        public List<Interval> Intervals
        {
            get => _intervals;
            set => _intervals = value;
        }

        // =======================================================================
        public class Interval
        {
            public readonly int   Index;
            public readonly float Start;
            public readonly float Lenght;

            public Interval(int index, float start, float length)
            {
                Index = index;
                Start = start;
                Lenght = length;
            }
        }

        // =======================================================================
        public bool Move(float step, out Interval[] moves)
        {
            var move = _move().ToArray();
            
            _position = (_position + step.PositivePart()).Clamp(0, _current.Start + _current.Lenght);
            
            if (move.IsEmpty() == false)
            {
                moves = move;
                return true;
            }
            
            moves = null;
            return false;

            // -----------------------------------------------------------------------
            IEnumerable<Interval> _move()
            {
                for (var index = _current.Index; index < _intervals.Count - 1; index++)
                {
                    var next = _intervals[index + 1];
                    
                    var cost = next.Start - _position;
                    if (cost > step)
                    {
                        _position += step;
                        step -= cost;
                        break;
                    }
                    
                    // pay the cost, move next
                    _position += cost;
                    step -= cost;
                    _current = next;
                    
                    yield return _current;
                }
            }
        }
        
        public Interval IntervalAt(float position)
        {
            if (_intervals.IsEmpty())
                return null;

            if (position <= .0f)
                return _intervals.First();

            // block in interval or last
            var pos = .0f;
            foreach (var interval in _intervals)
            {
                pos += interval.Lenght;
                if (pos >= position)
                    return interval;
            }

            return _intervals.Last();
        }

        public void Init(float pos, params float[] intervals)
        {
            _data = intervals;
            
            _intervals = new List<Interval>(intervals.Length);

            var start = .0f;
            for (var index = 0; index < intervals.Length; index++)
            {
                var length = intervals[index];
                _intervals.Add(new Interval(index, start, length));
                start += length;
            }

            _position = pos;
            _current  = IntervalAt(pos);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Init(0, _data);
        }
    }
}