using CoreLib;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TMP_AnimationJitter : TMP_Animation.Module 
    {
        public override int        Order => 0;
        public override string     Link  => "Jitter";
        
        public float _offset;
        public float _rotation;

        // =======================================================================
        public override void Apply(TMP_TextInfo textInfo1, int to, int from)
        {
            var textInfo = Text.textInfo;

            for (var n = from; n < to; n++)
            {
                var materialIndex = textInfo.characterInfo[n].materialReferenceIndex;
                var vertexIndex   = textInfo.characterInfo[n].vertexIndex;
                var infoSource    = textInfo.meshInfo[materialIndex];
                var center        = (infoSource.vertices[vertexIndex + 0] + infoSource.vertices[vertexIndex + 1] + infoSource.vertices[vertexIndex + 2] + infoSource.vertices[vertexIndex + 3]) / 4f;
                
                var offset        = new Vector3(Random.Range(0f, _offset) - _offset * .5f, Random.Range(0f, _offset) - _offset * .5f, 0f);
                var rotation      = Random.Range(0f, _rotation) - _rotation * .5f;
                
                infoSource.vertices[vertexIndex + 0] += offset;
                infoSource.vertices[vertexIndex + 1] += offset;
                infoSource.vertices[vertexIndex + 2] += offset;
                infoSource.vertices[vertexIndex + 3] += offset;
                
                infoSource.vertices[vertexIndex + 0] = (infoSource.vertices[vertexIndex + 0] - center).RotateXY(rotation) + center;
                infoSource.vertices[vertexIndex + 1] = (infoSource.vertices[vertexIndex + 1] - center).RotateXY(rotation) + center;
                infoSource.vertices[vertexIndex + 2] = (infoSource.vertices[vertexIndex + 2] - center).RotateXY(rotation) + center;
                infoSource.vertices[vertexIndex + 3] = (infoSource.vertices[vertexIndex + 3] - center).RotateXY(rotation) + center;
            }
        }
    }
}