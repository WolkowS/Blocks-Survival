using UnityEngine;

namespace CoreLib.Sound
{
    public static class Utils
    {
        private static AudioSound s_PlaySoundAudio = new AudioSound();

        // =======================================================================
        private class AudioSound : IAudio
        {
            public AudioClip Clip   { get; set; }
            public float     Volume { get; set; }

            public void Play(IAudioContext context)
            {
                context.AudioSource.PlayOneShot(Clip, Volume);
            }

            public void Play(IAudioContext ctx, float vol)
            {
                throw new System.NotImplementedException();
            }
        }

        // =======================================================================
        public static void Play(this SoundPlayer player, in SoundAsset audioProvider, float volume)
        {
            if (audioProvider.IsNull())
                return;

            _playAudio(player, audioProvider.Audio, volume);
        }

        private static void _playAudio(SoundPlayer player, IAudio audio, float volume)
        {
            if (audio.IsNull())
                return;

            s_PlaySoundAudio.Clip   = audio.Clip;
            s_PlaySoundAudio.Volume = volume;

            player.PlayAudio(s_PlaySoundAudio);
        }

        public static void Play(this SoundPlayer player, IAudio audio)
        {
            player.PlayAudio(audio);
        }
    }
}