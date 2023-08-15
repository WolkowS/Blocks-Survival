using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using CoreLib.Values;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TMP_AnimationIn : TMP_Animation.Effect 
    {
        public  Vers<Settings> _default;
        private List<float>    _time = new List<float>();
        
        [NonSerialized]
        public  Optional<Settings> _override = new Optional<Settings>();

        // =======================================================================
        [Serializable]
        public class Settings
        {
            public  Optional<AnimationCurve> _scale;
            public  Optional<AnimationCurve> _rotation;
            public  Optional<AnimationCurve> _offset;
            [ShowIf(nameof(_offset))]
            public Vector2                   _offsetDir = Vector2.up;
        }
        
        // =======================================================================
        public override void Apply(TMP_TextInfo textInfo)
        {
            var settings = _override.GetValueOrDefault(_default.Value);
            
            for (var n = 0; n < textInfo.characterCount; n++)
            {
                if (_time.Count <= n)
                    break;
                
                if (_time[n] >= float.MaxValue)
                    continue;
                
                var time          = Time.time - _time[n];
                var materialIndex = textInfo.characterInfo[n].materialReferenceIndex;
                var vertexIndex   = textInfo.characterInfo[n].vertexIndex;
                var infoSource    = textInfo.meshInfo[materialIndex];
                if (infoSource.vertices.Length <= vertexIndex + 3)
                    break;
                
                var center        = (infoSource.vertices[vertexIndex + 0] + infoSource.vertices[vertexIndex + 1] + infoSource.vertices[vertexIndex + 2] + infoSource.vertices[vertexIndex + 3]) / 4f;
                
                if (settings._rotation.Enabled)
                {
                    var rotation = settings._rotation.Value.Evaluate(time);
                    
                    infoSource.vertices[vertexIndex + 0] = (infoSource.vertices[vertexIndex + 0] - center).RotateXY(rotation) + center;
                    infoSource.vertices[vertexIndex + 1] = (infoSource.vertices[vertexIndex + 1] - center).RotateXY(rotation) + center;
                    infoSource.vertices[vertexIndex + 2] = (infoSource.vertices[vertexIndex + 2] - center).RotateXY(rotation) + center;
                    infoSource.vertices[vertexIndex + 3] = (infoSource.vertices[vertexIndex + 3] - center).RotateXY(rotation) + center;
                }
                
                if (settings._scale.Enabled)
                {
                    var scale = settings._scale.Value.Evaluate(time);

                    infoSource.vertices[vertexIndex + 0] = (infoSource.vertices[vertexIndex + 0] - center) * scale + center;
                    infoSource.vertices[vertexIndex + 1] = (infoSource.vertices[vertexIndex + 1] - center) * scale + center;
                    infoSource.vertices[vertexIndex + 2] = (infoSource.vertices[vertexIndex + 2] - center) * scale + center;
                    infoSource.vertices[vertexIndex + 3] = (infoSource.vertices[vertexIndex + 3] - center) * scale + center;
                }
                
                if (settings._offset.Enabled)
                {
                    var offsetVal = settings._offset.Value.Evaluate(time);
                    var offset = new Vector3(settings._offsetDir.x * offsetVal, settings._offsetDir.y * offsetVal);
                    
                    infoSource.vertices[vertexIndex + 0] += offset;
                    infoSource.vertices[vertexIndex + 1] += offset;
                    infoSource.vertices[vertexIndex + 2] += offset;
                    infoSource.vertices[vertexIndex + 3] += offset;
                }
            }
        }
        
        public override void Revealed(int index)
        {   
            _time.SetValue(index, Time.time);
        }

        public override void Rebuild()
        {
            _time.Clear();
            _time.AddRange(Enumerable.Repeat(float.MaxValue, Text.text.Length + 1));
        }
    }
}