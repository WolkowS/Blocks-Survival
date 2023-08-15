using System;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TextEffect : MonoBehaviour
    {
        //public Operation<TMP_Text> _text;
        [AutoSet]
        private TMP_Text _text;

        [SerializeField]
        private TextEffectAsset _effect;

        private TMP_VertexDataUpdateFlags _flags;

        public TextEffectAsset Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                _updateFlags();
            }
        }

        // =======================================================================
        private void Awake()
        {
            _updateFlags();
        }

        private void _updateFlags()
        {
            _flags = TMP_VertexDataUpdateFlags.None;

            if (_effect.ColorEffect) _flags  |= TMP_VertexDataUpdateFlags.Colors32;
            if (_effect.VertexEffect) _flags |= TMP_VertexDataUpdateFlags.Vertices;
        }

        private void Update()
        {
            _text.ForceMeshUpdate();

            // Loops each link tag
            foreach (var link in _text.textInfo.linkInfo)
            {
                // Is it a rainbow tag? (<link="rainbow"></link>)
                if (link.GetLinkID() == _effect._linkTag)
                {
                    // Loops all characters containing the rainbow link.
                    for (var n = link.linkTextfirstCharacterIndex; n < link.linkTextfirstCharacterIndex + link.linkTextLength; n++)
                    {
                        var charInfo = _text.textInfo.characterInfo[n];      // Gets info on the current character
                        if (charInfo.character == ' ') continue;             // Skips spaces
                        if (charInfo.isVisible == false) continue;           // Skips hidden
                        var materialIndex = charInfo.materialReferenceIndex; // Gets the index of the current character material

                        var colors   = _text.textInfo.meshInfo[materialIndex].colors32;
                        var vertices = _text.textInfo.meshInfo[materialIndex].vertices;

                        _effect.Setup(in link);

                        var index = charInfo.vertexIndex;
                        
                        if (_flags.HasFlags(TMP_VertexDataUpdateFlags.Colors32))
                            _effect.ApplyColor(in charInfo, ref colors[index], ref colors[index + 1], ref colors[index + 2], ref colors[index + 3]);
                        
                        if (_flags.HasFlags(TMP_VertexDataUpdateFlags.Vertices))
                            _effect.ApplyVertex(in charInfo, ref vertices[index], ref vertices[index + 1], ref vertices[index + 2], ref vertices[index + 3]);
                    }
                }
            }

            _text.UpdateVertexData(_flags);
        }
    }
}