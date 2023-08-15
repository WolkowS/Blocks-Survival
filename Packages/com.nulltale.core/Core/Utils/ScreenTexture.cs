using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class ScreenTexture : MonoBehaviour
    {
        [SerializeField] [ReadOnly]
        private Texture2D m_Texture;
        public Texture2D Texture => m_Texture;

        [SerializeField]
        private UnityEvent<Sprite> m_OnInvoke;

        private Sprite m_Sprite;

        // =======================================================================
        [Button]
        public void Invoke()
        {
            this.LateUpdate(_invoke);

            // -----------------------------------------------------------------------
            void _invoke()
            {
                if (m_Texture)
                {
                    Destroy(m_Sprite);
                    Destroy(m_Texture);
                }

                m_Texture            = ScreenCapture.CaptureScreenshotAsTexture();
                m_Texture.filterMode = FilterMode.Point;
                m_Sprite             = Sprite.Create(m_Texture, m_Texture.GetRect(), new Vector2(0.5f, 0.5f), Mathf.Max(m_Texture.width, m_Texture.height));

                m_OnInvoke.Invoke(m_Sprite);
            }
        }
    }
}