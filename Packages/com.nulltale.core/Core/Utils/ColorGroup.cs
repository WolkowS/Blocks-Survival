using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    [DefaultExecutionOrder(2)] [ExecuteAlways]
    public class ColorGroup : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)]
        private float             m_Weight = 1.0f;
        public  Color             m_Color;
        private Color             m_ColorPrev;
        private float             m_WeightPrev = -1f;
        private List<LinkAdapter> m_Links      = new List<LinkAdapter>();
        
        public bool m_Sprites = true;
        public bool m_Images = true;
        public bool m_Text = true;

        protected Color Color => Color.Lerp(Color.white, m_Color, m_Weight);
        
        // =======================================================================
        public abstract class LinkAdapter
        {
            public    ColorGroup m_Owner;

            protected Color Color => m_Owner.Color;
            
            public abstract void Apply();
        }

        private class LinkImage : LinkAdapter
        {
            private Image m_Image;

            public override void Apply()
            {
                if (m_Image == null)
                    return;

                m_Image.color = Color;
            }

            public LinkImage(Image image)
            {
                m_Image = image;
            }
        }

        private class LinkSprite : LinkAdapter
        {
            private SpriteRenderer m_Sprite;

            public override void Apply()
            {
                if (m_Sprite == null)
                    return;

                m_Sprite.color = Color;
            }

            public LinkSprite(SpriteRenderer sprite)
            {
                m_Sprite = sprite;
            }
        }
        
        private class LinkTextMeshPro : LinkAdapter
        {
            private TMP_Text m_Text;

            public override void Apply()
            {
                if (m_Text == null)
                    return;
                
                m_Text.color = Color;
            }

            public LinkTextMeshPro(TMP_Text text)
            {
                m_Text = text;
            }
        }

        // =======================================================================
        private void Start()
        {
            Link();
            Update();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                Link();
#endif
        }

        private void Update()
        {
            if (m_Weight == m_WeightPrev && m_Color == m_ColorPrev)
                return;
            
            foreach (var link in m_Links)
                link.Apply();

            m_WeightPrev = m_Weight;
            m_ColorPrev  = m_Color;
        }

        public void Link()
        {
            m_Links.Clear();

            if (m_Sprites)
                m_Links.AddRange(GetComponentsInChildren<Image>().Select(n => new LinkImage(n){m_Owner = this}));
            
            if (m_Images)
                m_Links.AddRange(GetComponentsInChildren<SpriteRenderer>().Select(n => new LinkSprite(n){m_Owner = this}));
            
            if (m_Text)
                m_Links.AddRange(GetComponentsInChildren<TMP_Text>().Select(n => new LinkTextMeshPro(n){m_Owner = this}));
        }
    }
}