using CoreLib.Fx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class ColorAdapter
    {
        public Color Color
        {
            set => m_ColorAdapter.Color = value;
            get => m_ColorAdapter.Color;
        }

        private IColorAdapter m_ColorAdapter;
            
        // =======================================================================
        private interface IColorAdapter
        {
            Color Color { get; set; }
        }

        private class SpriteAdapter : IColorAdapter
        {
            private SpriteRenderer m_Sprite;

            public Color Color
            {
                get => m_Sprite.color;
                set => m_Sprite.color = value;
            }

            public SpriteAdapter(SpriteRenderer sprite)
            {
                m_Sprite = sprite;
            }
        }

        private class ImageAdapter : IColorAdapter
        {
            private Image m_Image;

            public Color Color
            {
                get => m_Image.color;
                set => m_Image.color = value;
            }

            public ImageAdapter(Image image)
            {
                m_Image = image;
            }
        }
            
        private class LineAdapter : IColorAdapter
        {
            private LineRenderer m_Line;

            public Color Color
            {
                get => m_Line.startColor;
                set
                {
                    m_Line.startColor = value; 
                    m_Line.endColor   = value;
                }
            }

            public LineAdapter(LineRenderer line)
            {
                m_Line = line;
            }
        }
            
        private class TextAdapter : IColorAdapter
        {
            private TMP_Text m_Text;

            public Color Color
            {
                get => m_Text.color;
                set => m_Text.color = value;
            }

            public TextAdapter(TMP_Text text)
            {
                m_Text = text;
            }
        }

        private class ScreenOverlayAdapter : IColorAdapter
        {
            private ScreenOverlay m_ScreenOverlay;

            public Color Color
            {
                get => m_ScreenOverlay.m_Color;
                set => m_ScreenOverlay.m_Color = value;
            }

            public ScreenOverlayAdapter(ScreenOverlay screenOverlay)
            {
                m_ScreenOverlay = screenOverlay;
            }
        }
            
        // =======================================================================
        public ColorAdapter(GameObject root)
        {
            if (root.TryGetComponent(out SpriteRenderer sprite))
                m_ColorAdapter = new SpriteAdapter(sprite);
            else
            if (root.TryGetComponent(out Image image))
                m_ColorAdapter = new ImageAdapter(image);
            else
            if (root.TryGetComponent(out LineRenderer line))
                m_ColorAdapter = new LineAdapter(line);
            else
            if (root.TryGetComponent(out ScreenOverlay screenOverlay))
                m_ColorAdapter = new ScreenOverlayAdapter(screenOverlay);
            else
            if (root.TryGetComponent(out TMP_Text text))
                m_ColorAdapter = new TextAdapter(text);
        }
    }
}