using System.IO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace CoreLib.Sound
{
    public class Sound : SoundAsset, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clip;

        [Range(0, 1)]
        public float     m_Volume = 1;
        public Optional<float> m_Cooldown;
        public AudioClip m_Clip;
        
        private float m_LastInvoke;

        // =======================================================================
        public void Play(IAudioContext ctx)
        {
            if (m_Clip == null)
                return;

            if (_isReady() == false) 
                return;

            ctx.AudioSource.PlayOneShot(m_Clip, m_Volume);
        }

        public void Play(IAudioContext ctx, float vol)
        {
            if (_isReady() == false) 
                return;
            
            ctx.AudioSource.PlayOneShot(m_Clip, m_Volume * vol);
        }

        private bool _isReady()
        {
            if (m_Cooldown.Enabled)
            {
                var lastInvoke = m_LastInvoke;
                m_LastInvoke = Time.unscaledTime;

                if (Time.unscaledTime - lastInvoke < m_Cooldown.Value)
                    return false;
            }
            
            return true;
        }
#if UNITY_EDITOR
        [Button]
        public void SaveWav()
        {
            var folder = Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this));
            var path   = folder + $"\\Prototype\\{name}.wav";
            
            SavWav.Save(path, Clip);
            UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
            //UnityEditor.AssetDatabase.Refresh();
            
            m_Clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}