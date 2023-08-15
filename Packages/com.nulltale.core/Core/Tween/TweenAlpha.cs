using CoreLib.Fx;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Tween
{
    public class TweenAlpha : Tween
    {
        private float          m_Impact;
        private IAlphaAdapter  m_AlphaAdapter;

        // =======================================================================
        private interface IAlphaAdapter
        {
            bool  IsValid { get; }
            float Alpha   { get; set; }
        }

        private class SpriteAdapter : IAlphaAdapter
        {
            private SpriteRenderer m_Sprite;

            public bool IsValid => m_Sprite != null;

            public float Alpha
            {
                get => m_Sprite.color.a;
                set => m_Sprite.color = m_Sprite.color.WithA(value);
            }

            public SpriteAdapter(SpriteRenderer sprite)
            {
                m_Sprite = sprite;
            }
        }

        private class ImageAdapter : IAlphaAdapter
        {
            private Image m_Image;

            public bool IsValid => m_Image != null;
            
            public float Alpha
            {
                get => m_Image.color.a;
                set => m_Image.color = m_Image.color.WithA(value);
            }

            public ImageAdapter(Image image)
            {
                m_Image = image;
            }
        }

        private class CanvasGroupAdapter : IAlphaAdapter
        {
            private CanvasGroup m_CanvasGroup;

            public bool IsValid => m_CanvasGroup != null;
            public float Alpha
            {
                get => m_CanvasGroup.alpha;
                set => m_CanvasGroup.alpha = value;
            }

            public CanvasGroupAdapter(CanvasGroup canvasGroup)
            {
                m_CanvasGroup = canvasGroup;
            }
        }

        private class ScreenOverlayAdapter : IAlphaAdapter
        {
            private ScreenOverlay m_ScreenOverlay;

            public bool IsValid => m_ScreenOverlay != null;
            public float Alpha
            {
                get => m_ScreenOverlay.m_Color.a;
                set => m_ScreenOverlay.m_Color = m_ScreenOverlay.m_Color.WithA(value);
            }

            public ScreenOverlayAdapter(ScreenOverlay screenOverlay)
            {
                m_ScreenOverlay = screenOverlay;
            }
        }
        
        // =======================================================================
        private void Awake()
        {
            var root = m_Root.GetValueOrDefault(gameObject);
            
            if (root.TryGetComponent(out SpriteRenderer sprite))
                m_AlphaAdapter = new SpriteAdapter(sprite);
            else
            if (root.TryGetComponent(out Image image))
                m_AlphaAdapter = new ImageAdapter(image);
            else
            if (root.TryGetComponent(out CanvasGroup canvasGroup))
                m_AlphaAdapter = new CanvasGroupAdapter(canvasGroup);
            else
            if (root.TryGetComponent(out ScreenOverlay screenOverlay))
                m_AlphaAdapter = new ScreenOverlayAdapter(screenOverlay);
        }

        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying == false && m_AlphaAdapter == null)
                Awake();
#endif
            
            m_AlphaAdapter.Alpha = m_AlphaAdapter.Alpha + impact - m_Impact;
            m_Impact       = impact;
        }

        public override void Revert()
        {
            if (m_AlphaAdapter.IsValid)
                m_AlphaAdapter.Alpha -= m_Impact;
            
            m_Impact = 0f;
        }
    }
}