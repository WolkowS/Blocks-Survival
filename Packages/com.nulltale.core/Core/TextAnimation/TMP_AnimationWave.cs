using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    public class TMP_AnimationWave : TMP_Animation.Module 
    {
        public override int    Order => 0;
        public override string Link  => "Wave";
        
        public float _height;
        public float _period;
        public float _speed;

        // =======================================================================
        public override void Apply(TMP_TextInfo textInfo, int to, int from)
        {
            var period = (Time.time * _speed) % _period;

            for (var n = from; n < to; n++)
            {
                var xAdvance      = textInfo.characterInfo[n].xAdvance;
                var materialIndex = textInfo.characterInfo[n].materialReferenceIndex;
                var vertexIndex   = textInfo.characterInfo[n].vertexIndex;
                var infoSource    = textInfo.meshInfo[materialIndex];
                var offset        = new Vector3(0f, (Mathf.Sin(xAdvance / _period + period) + 1f) * _height, 0f);
                
                infoSource.vertices[vertexIndex + 0] += offset;
                infoSource.vertices[vertexIndex + 1] += offset;
                infoSource.vertices[vertexIndex + 2] += offset;
                infoSource.vertices[vertexIndex + 3] += offset;
            }
        }
    }
}