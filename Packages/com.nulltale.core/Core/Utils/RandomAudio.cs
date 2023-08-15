using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class RandomAudio : MonoBehaviour
    {
        public bool _notRepeat;
        public AudioClip[] _options;
        public UnityEvent<AudioClip> _onInvoke;
        
        private AudioClip _lastOption;
        
        // =======================================================================
        public void Invoke()
        {
            _lastOption = (_options.Length > 1 && _notRepeat) ? _options.Except(_lastOption).Random() : _options.Random();
            _onInvoke.Invoke(_lastOption);
        }
    }
}