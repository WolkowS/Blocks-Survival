using CoreLib;
using CoreLib.Fx;
using CoreLib.Values;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class ColorState : MonoBehaviour, IToggle
    {
        public bool        _state;
        [Label("On")]
        public Vers<Color> _b;
        [Label("Off")]
        public Vers<Color> _a;

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                switch (_state)
                {
                    case false:
                        _assign(_a.Value);
                        break;
                    case true:
                        _assign(_b.Value);
                        break;
                }
            }
        }
        
        public bool IsOn => _state;
        
        // =======================================================================
        private void Awake()
        {
            State = _state;
        }
        
        private void _assign(Color color)
        {
            if (TryGetComponent(out SpriteRenderer sprite))
                sprite.color = color;
            else
            if (TryGetComponent(out Image image))
                image.color = color;
            else
            if (TryGetComponent(out TMP_Text text))
                text.color = color;
            else
            if (TryGetComponent(out ScreenOverlay screenOverlay))
                screenOverlay.m_Color = color;
        }
        
        public void On()
        {
            State = true;
        }

        public void Off()
        {
            State = false;
        }
    }
}