using CoreLib.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    [RequireComponent(typeof(SoundPlayer))]
    public class UIButtonSound : MonoBehaviour
    {
        public SoundAsset m_Sound;
        public SoundAsset m_Hover;

        // =======================================================================
        private void Awake()
        {
            var audioPlayer = GetComponent<SoundPlayer>();

            var button = GetComponentInParent<Button>();
            if (button != null)
            {
                if (m_Sound.IsNull() == false)
                    button.onClick.AddListener(() => audioPlayer.Play(m_Sound));

                if (m_Hover.IsNull() == false)
                    button.gameObject
                          .AddComponent<OnHover>()
                          ._onHover
                          .AddListener((b) =>
                          {
                              if (b) 
                                  audioPlayer.Play(m_Hover);
                          });

                return;
            }

            var toggle = GetComponentInParent<Toggle>();
            if (toggle != null)
            {
                if (m_Sound.IsNull() == false)
                    toggle.onValueChanged.AddListener(inOn => audioPlayer.Play(m_Sound));

                if (m_Hover.IsNull() == false)
                    toggle.gameObject
                          .AddComponent<OnHover>()
                          ._onHover
                          .AddListener((b) =>
                          {
                              if (b) 
                                  audioPlayer.Play(m_Hover);
                          });

                return;
            }
        }
    }
}