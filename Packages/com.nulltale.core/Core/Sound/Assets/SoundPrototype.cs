using System.IO;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreLib.Sound
{
    public class SoundPrototype : SoundAsset, IAudio
    {
        public override IAudio Audio => this;
        public AudioClip Clip
        {
            get
            {
                var samples = (int)(m_Clip.samples * (_range.y - _range.x));
                var clip    = AudioClip.Create(string.Empty, samples, m_Clip.channels, m_Clip.frequency, false);
                
                var buffer       = new float[samples];
                var sampleOffset = (int)(m_Clip.samples * _range.x);
                
                m_Clip.GetData(buffer, sampleOffset);
                clip.SetData(buffer, 0);
                
                return clip;
            }
        }

        [Range(0, 1)]
        public float m_Volume = 1;
        public AudioClip m_Clip;
        
        [MinMaxSlider(0, 1)]
        public Vector2 _range = new Vector2(0, 1); 
        
        // =======================================================================
        public void Play(IAudioContext ctx)
        {
            if (m_Clip == null)
                return;

            ctx.AudioSource.PlayOneShot(Clip, m_Volume);
        }

        public void Play(IAudioContext ctx, float vol)
        {
            ctx.AudioSource.PlayOneShot(Clip, m_Volume * vol);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Audio/From Clip")]
        public static void CreateFromAudio()
        {
            var clip = UnityEditor.Selection.activeObject as AudioClip;
            if (clip == null)
                return;
            
            var assetPath = AssetDatabase.GetAssetPath(clip);
            var sound     = ScriptableObject.CreateInstance<SoundPrototype>();
            sound.m_Clip  = clip;
            sound.name    = Path.GetFileNameWithoutExtension(assetPath);
                
            UnityEditor.AssetDatabase.CreateAsset(sound, Path.GetDirectoryName(assetPath) + "/" + sound.name + ".asset");
            UnityEditor.EditorGUIUtility.PingObject(sound);
            UnityEditor.Selection.SetActiveObjectWithContext(sound, sound);
        }
        
        [Button]
        public void SaveWav()
        {
            var folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            var path          = folder + $"\\Prototype\\{name}";
            
            SavWav.Save(path, Clip);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            m_Clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}