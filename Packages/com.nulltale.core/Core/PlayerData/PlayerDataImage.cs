using System;
using System.IO;
using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataImage : PlayerData.DataHandle
    {
        [SerializeField]
        private Texture2D m_Value;
        [SerializeField]
        private Texture2D m_Default;
        
        [SerializeField]
        private ImageOptions    m_ImageOptions;

        [SerializeField]
        private  SaveFormat   m_SaveFormat;

        public Texture2D Value
        {
            get => m_Value;
            set
            {
                if (value == null)
                    m_Value = null;
                else
                {
                    if (m_ImageOptions.Size.Enabled)
                        m_Value = value.Copy(m_ImageOptions.Size.Value.x, m_ImageOptions.Size.Value.y);
                    else
                        m_Value = value.Copy();

                    m_Value.wrapMode   = m_ImageOptions.WrapMode;
                    m_Value.filterMode = m_ImageOptions.FilterMode;
                }
            }
        }

        private string ImageFileName => $"{name}{m_SaveFormat switch { SaveFormat.PNG => ".png", SaveFormat.JPEG => ".jpeg", _ => throw new ArgumentOutOfRangeException()}}";

        // =======================================================================
        [Serializable]
        public enum SaveFormat
        {
            PNG,
            JPEG,
        }

        [Serializable]
        public class ImageOptions
        {
            public Optional<Vector2Int> Size;
            public TextureWrapMode      WrapMode = TextureWrapMode.Clamp;
            public FilterMode           FilterMode = FilterMode.Point;
        }

        // =======================================================================
        public override void InitDefault()
        {
            m_Value = m_Default?.Copy();
        }

        public override void Init(string data)
        {
            var imagePath = $"{PlayerData.Instance.PlayerFolder}{ImageFileName}";

            try
            {
                if (File.Exists(imagePath))
                {
                    var fileData = File.ReadAllBytes(imagePath);
                    m_Value = new Texture2D(2, 2);
                    m_Value.LoadImage(fileData);
                    m_Value.wrapMode   = TextureWrapMode.Clamp;
                    m_Value.filterMode = FilterMode.Point;
                }
            }
            catch (Exception e)
            {
                InitDefault();
                Debug.LogWarning(e);
            }
        }

        public override string Serialize()
        {
            var imagePath = $"{PlayerData.Instance.PlayerFolder}{ImageFileName}";
            if (m_Value != null)
            {
                File.WriteAllBytes(imagePath, m_SaveFormat switch
                {
                    SaveFormat.PNG  => m_Value.EncodeToPNG(),
                    SaveFormat.JPEG => m_Value.EncodeToJPG(),
                    _               => throw new ArgumentOutOfRangeException()
                });
            }
            else
            {
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            return ImageFileName;
        }
    }
}