using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscAudioSpectrum : OscillatorBase
    {
        public Vers<AudioAnalizer> _audio;
        [RangeVec2]
        public Vector2             _output = new Vector2(0, 1);
        public Band                _band;
        [MinMaxSlider(0, 22050)]
        public Vector2Int          _rangeHz;
        public  Optional<float>    _smooth;
        private float              _smoothVel;
        private float              _value;
        private float              _timeLast;
        
        public override float Value
        {
            get
            {
                var val = _audio.Value.GetBandMax(_rangeHz.x, _rangeHz.y);
                if (_smooth.Enabled)
                {
                    var delta = Time.time - _timeLast;
                    if (delta != 0f)
                    {
                        _value    = Mathf.SmoothDamp(_value, val, ref _smoothVel, _smooth.Value, Mathf.Infinity, delta);
                        _timeLast = Time.time;
                    }
                }
                else
                {
                    _value = val;
                }
                
                return _output.x + _value * (_output.y - _output.x);
            }
        }

        // =======================================================================
        public enum Band
        {
            SubBass,
            Bass,
            LowMidrage,
            Midrange,
            HightMidrange,
            Presence,
            Brilliance,
            
            Custom,
        }
        
        // =======================================================================

        internal override void OnValidate()
        {
            base.OnValidate();

            _rangeHz = _band switch
            {
                Band.SubBass       => new Vector2Int(16, 60),
                Band.Bass          => new Vector2Int(60, 250),
                Band.LowMidrage    => new Vector2Int(250, 500),
                Band.Midrange      => new Vector2Int(500, 2000),
                Band.HightMidrange => new Vector2Int(2000, 4000),
                Band.Presence      => new Vector2Int(4000, 6000),
                Band.Brilliance    => new Vector2Int(6000, 22050),
                
                Band.Custom        => _rangeHz,
                
                _                  => throw new ArgumentOutOfRangeException()
            };
        }
    }
}