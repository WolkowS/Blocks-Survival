using CoreLib.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIToggleSound : MonoBehaviour
    {
        public SoundAsset m_Sound;
        public bool       m_ActivateOnly;

        // =======================================================================
        private void Start()
        {
            GetComponentInParent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (m_ActivateOnly && isOn == false)
                    return;
                
                m_Sound.Play();
            });
        }
    }
}