using NaughtyAttributes;
using UnityEngine;
using SoCreator;

namespace CoreLib.Sound
{
    [SoCreate(true)]
    public abstract class SoundAsset : ScriptableObject, IAudioProvider
    {
        public          string Key   => name;
        public abstract IAudio Audio { get; }

        public virtual void Init() {}
        
        [Button]
        public void Play()
        {
            if (Application.isPlaying == false)
            {
                Debug.LogError("Can't play audio in edit mode");
                return;
            }
            
            Audio?.Play(SoundManager.Instance);
        }
        
        public void Play(float vol)
        {
            if (Application.isPlaying == false)
            {
                Debug.LogError("Can't play audio in edit mode");
                return;
            }
            
            Audio?.Play(SoundManager.Instance, vol);
        }
    }
}