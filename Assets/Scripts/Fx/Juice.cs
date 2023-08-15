using System;
using CoreLib;
using CoreLib.Sound;
using NaughtyAttributes;
using UnityEngine;

namespace Game.FX
{
    public class Juice : MonoBehaviour
    {
        public Optional<SlowmoAsset>        _slowmo;
        public Optional<ScreenShakeAsset>   _impulse;
        public Optional<ScreenOverlayAsset> _screen;
        public Optional<ScreenFrameAsset>   _frame;
        public Optional<ScreenRollAsset>    _roll;
        public Optional<SoundAsset>         _sound;
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            if(_slowmo)
                _slowmo.Value.Invoke();
            
            if(_impulse)
                _impulse.Value.Invoke();
            
            if(_screen)
                _screen.Value.Invoke();
            
            if(_frame)
                _frame.Value.Invoke();
            
            if(_roll)
                _roll.Value.Invoke();
            
            if(_sound)
                _sound.Value.Play();
        }
    }
}