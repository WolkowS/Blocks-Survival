using System;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenOverlayBehaviour : PlayableBehaviour
    {
        public float            m_Scale = 1f;
        public Color            m_Color = Color.black;
        public Optional<Sprite> m_Image;
        public bool             m_ScreenShot;
        
        // =======================================================================
        public void TakeScreen()
        {
            var tex = ScreenCapture.CaptureScreenshotAsTexture();
            tex.filterMode = FilterMode.Point;
            m_Image.Value  = Sprite.Create(tex, tex.GetRect(), new Vector2(0.5f, 0.5f), Mathf.Max(tex.width, tex.height));
        }
        
        public void ReleaseScreen()
        {
            if (m_Image.Value)
            {
                Object.Destroy(m_Image.Value.texture);
                Object.Destroy(m_Image.Value);
            }
        }
    }
}