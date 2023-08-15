using System;
using CoreLib.Values;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Scripting
{
    public class SetColor : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _root;
        public Vers<Color>                _color;
        
        // =======================================================================
        public void Invoke()
        {
            var root = _root.Enabled ? _root.Value.Value : gameObject;
            
            if (root.TryGetComponent(out SpriteRenderer sprite))
                sprite.color = _color.Value;
            else
            if (root.TryGetComponent(out Image image))
                image.color = _color.Value;
            else
            if (root.TryGetComponent(out TMP_Text text))
                text.color = _color.Value;
        }
    }
}