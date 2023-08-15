using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [ExecuteAlways]
    public class UITextTyper : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_Text;

        [SerializeField] [Range(0, 1)]
        private float m_Progress;
        public float Progress
        {
            get => m_Progress;
            set
            {
                var valueNormalized = value.Clamp01();
                if (m_Progress == valueNormalized)
                    return;

                m_Progress = valueNormalized;
                _update();
            }
        }

        public int VisibleCharacters
        {
            get => (m_Progress * m_Text.textInfo.characterCount).RoundToInt();
            set => Progress = value / (float)m_Text.textInfo.characterCount;
        }

        public float TypeSpeed
        {
            get => m_TypeSpeed;
            set => m_TypeSpeed = value;
        }

        [SerializeField]
        private float m_TypeSpeed;

        private int m_Visible = -1;
        public UnityEvent   m_OnType;

        // =======================================================================
        private void _update()
        {
            var textInfo = m_Text.textInfo;
            var visible  = VisibleCharacters;
            if (m_Visible == visible)
                return;

            if (m_Visible < visible)
                m_OnType.Invoke();

            m_Visible = visible;

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

            m_Text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void Update()
        {
            if (Application.isPlaying == false)
                return;
            
            Progress += (1f / m_Text.textInfo.characterCount) * m_TypeSpeed * Time.unscaledDeltaTime;
            _update();
        }

        private void OnValidate()
        {
            try
            {
                _update();
            }
            catch
            {
                // pass
            }
        }
    }
}