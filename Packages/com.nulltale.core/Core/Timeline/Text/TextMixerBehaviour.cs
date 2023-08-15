using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class TextMixerBehaviour : PlayableBehaviour
    {
        public const float k_PunctuationFallback = 1f;
        public static Dictionary<char, float> k_PunctuationDefault = new Dictionary<char, float>()
        {
            { ' ', 2.1f },
            { ',', 2.1f },
            { '.', 2.1f },
            { '?', 2.1f },
            { '!', 2.1f },
        };
        
        public  TextTrack.Methotd _method;
        private string            _initial;
        private TMP_Text          _text;

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_text != null)
            {
                _text.text = _initial;
                _text = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_text == null)
            {
                _text    = (TMP_Text)playerData;
                if (_text == null)
                    _text = TextTrackOutput.Instance.Text;
                
                _initial = _text.text;
            }
            
            var inputCount = playable.GetInputCount();

            // calculate weights
            var fullWeight = 0f;

            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                if (inputWeight <= 0f)
                    continue;

                fullWeight += inputWeight;

                var inputPlayable = (ScriptPlayable<TextBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();
                
                if (inputWeight > 0)
                {
                    switch (behaviour.m_Source)
                    {
                        case TextAsset.TextSource.Content:
                        {
                            _text.text = behaviour.m_Text;
                        } break;
                        case TextAsset.TextSource.Initial:
                        {
                            _text.text = _initial;
                        } break;
                        case TextAsset.TextSource.GlobalValue:
                        {
                            _text.text = behaviour.m_Value.Value;
                        } break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            var textWeight  = _text.text.Sum(n => k_PunctuationDefault.TryGetValue(n, out var weight) ? weight : k_PunctuationFallback);
            var goalWeight  = textWeight * fullWeight;
            var trackWeight = 0f;
            
            foreach (var c in _text.text)
            {
                var w = k_PunctuationDefault.TryGetValue(c, out var weight) ? weight : k_PunctuationFallback;
                trackWeight += w;
                if (trackWeight >= goalWeight)
                {
                    if ((trackWeight - goalWeight) >= w * .5f)
                        trackWeight -= w;
                    
                    break;
                }
            }
            
            var textInfo = _text.textInfo;
            var visible  = ((trackWeight / textWeight) * textInfo.characterCount).RoundToInt();

            for (var n = 0; n < textInfo.characterCount; n++)
            {
                var characterInfo = textInfo.characterInfo[n];
                if (characterInfo.isVisible == false)
                    continue;

                var newVertexColors = textInfo.meshInfo[characterInfo.materialReferenceIndex].colors32;
                // get the index of the first vertex used by this text element.
                var vertexIndex = textInfo.characterInfo[n].vertexIndex;
                
                var alpha = n < visible ? byte.MaxValue : byte.MinValue;

                // set all to full alpha
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;
            }

            _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
        
    }
}