using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Sound
{
    public class SoundPlayerQueue : SoundPlayer
    {
        public  int           _capacity = 1;
        private float         _playNext;
        private Queue<IAudio> _queue = new Queue<IAudio>();
        private Coroutine     _coroutine;

        // =======================================================================
        public override void PlayAudio(IAudio audio)
        {
            if (_queue.Count > _capacity)
                return;
            
            if (_playNext <= Time.unscaledTime)
            {
                _play(audio);
            }
            else
            if (_coroutine == null)
            {
                _queue.Enqueue(audio);
                _coroutine = StartCoroutine(_update());
            }
        }

        private IEnumerator _update()
        {
            while (_queue.Count > 0)
            {
                while (_playNext >= Time.unscaledTime)
                    yield return null;
                
                var audioNext = _queue.Dequeue();
                _play(audioNext);
            }
            
            _coroutine = null;
        }

        private void _play(IAudio audio)
        {
            _playNext = Time.unscaledTime + audio.Clip.length;
            base.PlayAudio(audio);
        }

        private void OnDisable()
        {
            _coroutine = null;
            _queue.Clear();
        }
    }
}