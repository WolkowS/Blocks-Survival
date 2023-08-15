using CoreLib.Module;
using CoreLib.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreLib
{
    public class UIURLLinkOpener : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text   m_Text;

        [SerializeField]
        private SoundAsset m_ClickAudio;

        // =======================================================================
        private void Awake()
        {
            m_Text = GetComponentInChildren<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, Pointer.Screen.To3DXY(), 
                m_Text.canvas == null 
                    ? (Core.Camera != null 
                        ? Core.Camera 
                        : Camera.main)
                    : (m_Text.canvas.renderMode == RenderMode.ScreenSpaceOverlay 
                        ? null 
                        : m_Text.canvas.worldCamera)
            );

            if (linkIndex != -1)
            {
                var linkInfo = m_Text.textInfo.linkInfo[linkIndex];

                // play sound
                m_ClickAudio.Play();

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}