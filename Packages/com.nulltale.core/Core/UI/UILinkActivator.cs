using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CoreLib
{
    [RequireComponent(typeof(TMP_Text))]
    public class UILinkActivator : MonoBehaviour, IPointerClickHandler 
    {
        private TMP_Text m_Text;

        [SerializeField]
        private SerializableDictionaryBase<string, UnityEvent<string>> m_EventDictionary;

        // =======================================================================
        private void Awake()
        {
            m_Text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            Camera camera = null;
            if (m_Text.canvas == null)
                camera = Core.Camera != null ? Core.Camera : Camera.main;
            else
                camera = m_Text.canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_Text.canvas.worldCamera;

            var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, eventData.position, camera);

            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkIndex];

                // run event
                var linkID = linkInfo.GetLinkID();
                if (m_EventDictionary.TryGetValue(linkID, out var ultEvent))
                    ultEvent.Invoke(linkID);
                else
                    Debug.LogWarning("Link value not presented in the dictionary");
            }
        }
    }
}