using System;
using System.Collections.Generic;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class AudioAnalizer : MonoBehaviour
    {
        public  Optional<AudioSource> _audio;
        public  FFTWindow             _window = FFTWindow.Hamming;
        public  Optional<float>       _mul    = new Optional<float>(1f);
        public  bool                  _log;
        public  Size                  _size;
        public  Channel               _channel    = Channel.A;
        private float[]               _data       = new float[128];
        private float[]               _spectrum   = Array.Empty<float>();
        private int                   _lastUpdate = -1;
        
        public float[] Data
        {
            get
            {
                _update();
                return _data;
            }
        }

        // =======================================================================
        public enum Size
        {
            N128,
            N256,
            N512,
            N1024,
            N2048,
            N4096,
        }

        [Flags]
        public enum Channel
        {
            None   = 0,
            A      = 1,
            B      = 1 << 1,
            Stereo = -1,
        }
        
        // =======================================================================
        public float GetBandMax(int minHz, int maxHz)
        {
            _update();

            var perValueHz = AudioSettings.outputSampleRate / (float)_data.Length;
            if (maxHz >= AudioSettings.outputSampleRate)
                maxHz = AudioSettings.outputSampleRate - 1;

            var start  = (minHz / perValueHz).FloorToInt();
            var finish = (maxHz / perValueHz).FloorToInt();

            var result = 0f;
            for (var n = start; n < finish; n ++)
                result = Mathf.Max(result, _data[n]);
            
            return result;
        }
        
        public IEnumerable<float> GetBand(int minHz, int maxHz)
        {
            _update();
            
            var perValueHz = AudioSettings.outputSampleRate / (float)_data.Length;
            if (maxHz >= AudioSettings.outputSampleRate)
                maxHz = AudioSettings.outputSampleRate - 1;

            var start  = (minHz / perValueHz).FloorToInt();
            var finish = (maxHz / perValueHz).FloorToInt();

            for (var n = start; n < finish; n ++)
                yield return _data[n];
        }
        
        // =======================================================================
        public void _update()
        {
            if (_lastUpdate == Time.frameCount)
                return;
            
            _lastUpdate = Time.frameCount;
            
            var size = _size switch
            {
                Size.N128  => 128,
                Size.N256  => 256,
                Size.N512  => 512,
                Size.N1024 => 1024,
                Size.N2048 => 2048,
                Size.N4096 => 4096,
                _          => throw new ArgumentOutOfRangeException()
            };
            
            if (_spectrum.Length != size)
            {
                _spectrum = new float[size];
                _data     = new float[size];
            }

            var audio = _audio.Enabled ? _audio.Value : GetComponent<AudioSource>();
            switch (_channel)
            {
                case Channel.A:
                {
                    audio.GetSpectrumData(_data, 0, _window);
                } break;
                
                case Channel.B:
                {
                    audio.GetSpectrumData(_data, 1, _window);
                } break;
                
                case Channel.Stereo:
                {
                    audio.GetSpectrumData(_spectrum, 0, _window);
                    audio.GetSpectrumData(_data, 1, _window);
                    
                    for (var n = 0; n < _data.Length; n++)
                        _data[n] = Mathf.Max(_data[n], _spectrum[n]);
                } break;

                case Channel.None:
                {
                    _data.Clear();
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (_mul.Enabled)
            {
                for (var n = 0; n < _data.Length; n++) 
                    _data[n] = (_data[n] * _mul.Value).Clamp01();
            }
            
            if (_log)
            {
                for (var n = 0; n < _data.Length; n++) 
                    _data[n] = Mathf.Log(_data[n]);
            }
        }
    }
}