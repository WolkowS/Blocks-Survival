using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class RandomString : MonoBehaviour
    {
        public bool _notRepeat;
        [ResizableTextArea]
        public string[] _options;
        public UnityEvent<string> _onInvoke;
        
        private string _lastOption;
        
        // =======================================================================
        public void Invoke()
        {
            _lastOption = (_options.Length > 1 && _notRepeat) ? _options.Except(_lastOption).Random() : _options.Random();
            _onInvoke.Invoke(_lastOption);
        }
    }
}