using CoreLib.Module;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib
{
    public class ScreenValue<T> : MonoBehaviour
    {
        public Optional<string>      _name;
        public Vers<T>               _value;
        public Optional<Vers<Color>> _color = new Optional<Vers<Color>>(new Vers<Color>(Color.white));

        public T Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        // =======================================================================
        private void Update()
        {
            var text = _value.Value.ToString();
            
            if (_name.Enabled)
                text = _name.Value.IsNullOrEmpty() == false ? $"{_name.Value}: {text}" : text;
            else
                text = $"{name}: {text}";
            
            if(_color.Enabled)
                text = $"<color=#{ColorUtility.ToHtmlStringRGB(_color.Value.Value)}>{text}</color>";
            
            DebugTools.ScreenLog(text);
        }
    }
}