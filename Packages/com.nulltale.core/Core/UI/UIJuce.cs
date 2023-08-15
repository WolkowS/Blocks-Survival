using CoreLib.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIJuce : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
    
        public Optional<GameObject> m_Target;
        public float m_Duration   = 0.33f;
        public float m_PressScale = 0.04f;
        public float m_HoverScale = 0.1f;
        public Optional<SoundAsset> m_HoverAudio;
        public Optional<SoundAsset> m_PressAudio;
        public Optional<SoundAsset> m_ReleaseAudio;
        public Optional<SoundAsset> m_ClickAudio;

        private float m_DesiredScale;
        private bool  m_IsPressed;
        private bool  m_IsHover;
        private float m_Impact;
        private float m_CurrentTime;
        private GameObject m_CurrentTarget;

        public bool IsPressed
        {
            get => m_IsPressed;
            set
            {
                m_CurrentTime = 0f;
                m_IsPressed = value;
                if (m_IsPressed)
                {
                    if (m_PressAudio.Enabled)
                        m_PressAudio.Value.Play();
                }
                else
                {
                    if (m_ReleaseAudio.Enabled)
                        m_ReleaseAudio.Value.Play();
                }

                _updateState();
            }
        }

        public bool IsHover
        {
            get => m_IsHover;
            set
            {
                m_CurrentTime = 0f;
                m_IsHover = value;
                if (m_IsHover && m_IsPressed == false)
                    if (m_HoverAudio.Enabled)
                        m_HoverAudio.Value.Play();

                _updateState();
            }
        }

        // =======================================================================
        private void Awake()
        {
            m_CurrentTarget = m_Target.Enabled ? m_Target.Value : gameObject; 
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHover = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
        }

        private void Update()
        {
            if (m_Impact == m_DesiredScale)
                return;

            m_CurrentTime += Time.unscaledDeltaTime;
            var impact = m_Duration > 0f ? Mathf.Lerp(m_Impact, m_DesiredScale, m_CurrentTime / m_Duration) : m_DesiredScale;
            m_CurrentTarget.transform.localScale += (impact - m_Impact).ToVector3();
            m_Impact = impact;
        }

        private void OnDisable()
        {
            m_CurrentTarget.transform.localScale -= m_Impact.ToVector3();
            m_Impact = 0;
            m_DesiredScale = 0;
        }

        private void _updateState()
        {
            if (IsPressed)
            {
                m_DesiredScale = m_PressScale;
                return;
            }

            m_DesiredScale = m_IsHover ? m_HoverScale : 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_ClickAudio.Enabled)
                m_ClickAudio.Value.Play();
        }
    }
}