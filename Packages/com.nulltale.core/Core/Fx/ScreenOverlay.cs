using CoreLib.Module;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Fx
{
    [ExecuteAlways]
    public class ScreenOverlay : MonoBehaviour, Module.FxTools.IScreenOverlay
    {
        [Range(0, 1)]
        public float   m_Weight = 1f;
        public Color   m_Color;
        public Optional<Sprite>  m_Sprite;
        public Vector2 m_Scale = new Vector2(1.0f, 1.0f);

        protected Module.FxTools.ScreenOverlayHandle m_Handle;

        public Color   Color  { get => m_Color;  set => m_Color = value; }
        public Sprite  Sprite { get => m_Sprite; set => m_Sprite.Value = value; }
        public Image Image => m_Handle.Image;
        public Canvas Canvas => m_Handle.Canvas;
        public Vector2 Scale { get => m_Scale; set => m_Scale = value; }

        public float Weight
        {
            get => m_Weight;
            set => m_Weight = value;
        }

        // =======================================================================
        protected virtual void OnEnable()
        {
            m_Handle = new Module.FxTools.ScreenOverlayHandle();
            m_Handle.Open();
        }

        protected virtual void Update()
        {
            m_Handle.Color  = m_Color.WithA(m_Color.a * m_Weight);
            if (m_Sprite.Enabled)
                m_Handle.Sprite = m_Sprite;
            m_Handle.Scale  = m_Scale;
        }

        protected virtual void OnDisable()
        {
            m_Handle.Close();
            m_Handle.Dispose();
            m_Handle = null;
        }
    }
}