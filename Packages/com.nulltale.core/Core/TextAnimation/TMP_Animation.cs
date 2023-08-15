using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TMP_Animation : MonoBehaviour
    {
        private TMP_Text       _text;
        private Module[]       _modules;
        private Effect[]       _effects;

        private List<Pass>     _passes = new List<Pass>();
        private string         _textData;
        
        // =======================================================================
        private class Pass
        {
            public int    Start;
            public int    Finish;
            public Module Module;
        } 
        
        // =======================================================================
        public abstract class Effect : MonoBehaviour
        {
            public TMP_Animation Owner { get; set; }
            public TMP_Text            Text  { get; set; }
            
            // =======================================================================
            public abstract void Apply(TMP_TextInfo textInfo);
            public virtual void Revealed(int index) { }
            public virtual void Rebuild() { }
        }
        
        public abstract class Module : MonoBehaviour
        {
            public TMP_Animation Owner { get; set; }
            public TMP_Text            Text  { get; set; }
            
            public abstract int              Order { get; }
            public abstract string           Link  { get; }
            
            public Optional<string> _linkId;
        
            internal string GetLinkId() => _linkId.Enabled ? _linkId.Value : Link;  

            // -----------------------------------------------------------------------
            public abstract void Apply(TMP_TextInfo textInfo, int finish, int start);
            public virtual void Rebuild() { }
        }
        
        // =======================================================================
        private void Awake()
        {
            _text     = GetComponent<TMP_Text>();
            _modules  = GetComponents<Module>().OrderBy(n => n.Order).ToArray();
            _effects  = GetComponents<Effect>();

            foreach (var mod in _modules)
            {
                mod.Owner = this;
                mod.Text  = _text;
            }
            
            foreach (var effect in _effects)
            {
                effect.Owner = this;
                effect.Text  = _text;
            }
        }

        private void OnEnable()
        {
            _text.OnPreRenderText += _onPreRenderText;
        }
        
        private void OnDisable()
        {
            _text.OnPreRenderText -= _onPreRenderText;
        }

        private void _onPreRenderText(TMP_TextInfo textInfo)
        {
            if (_textData != _text.text)
                TextUpdate();
            
            foreach (var pass in _passes)
                pass.Module.Apply(textInfo, pass.Finish, pass.Start);
            
            foreach (var effect in _effects)
                effect.Apply(textInfo);
        }

        private void Update()
        {
            if (_passes.Count != 0 || _effects.Length != 0)
                _text.SetAllDirty();
        }

        public void TextUpdate()
        {
            _textData = _text.text;
            
            _passes.Clear();
            foreach (var linkInfo in _text.textInfo.linkInfo)
            {
                var mod = _modules.FirstOrDefault(n => n.GetLinkId() == linkInfo.GetLinkID());
                if (mod == null)
                    continue;
                
                _passes.Add(new Pass()
                {
                    Start  = linkInfo.linkTextfirstCharacterIndex,
                    Finish = linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength,
                    Module = mod 
                });
            }
            
            foreach (var mod in _modules)
                mod.Rebuild();
            
            foreach (var effect in _effects)
                effect.Rebuild();
        }
        
        public void OnGlyph(int revealed)
        {
            foreach (var effect in _effects)
                effect.Revealed(revealed);
        }
    }
}