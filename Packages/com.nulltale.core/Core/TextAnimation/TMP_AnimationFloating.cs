using CoreLib;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TMP_AnimationFloating : TMP_Animation.Module 
    {
        public override int    Order => 0;
        public override string Link  => "Floating";
        
        public float _range;
        public float _speed;

        // =======================================================================
        public override void Apply(TMP_TextInfo textInfo1, int to, int from)
        {
            var textInfo = Text.textInfo;

            for (var n = from; n < to; n++)
            {
                var characterInfo = textInfo.characterInfo[n];
                var materialIndex = characterInfo.materialReferenceIndex;
                var vertexIndex   = characterInfo.vertexIndex;
                var infoSource    = textInfo.meshInfo[materialIndex];
                
                var offset = (Mathf.Sin(characterInfo.xAdvance + Time.time * _speed) * Mathf.PI).NormalXY() * _range;
                
                infoSource.vertices[vertexIndex + 0] += offset;
                infoSource.vertices[vertexIndex + 1] += offset;
                infoSource.vertices[vertexIndex + 2] += offset;
                infoSource.vertices[vertexIndex + 3] += offset;
            }
        }
    }
}