using System;
using CoreLib;
using CoreLib.Scripting;
using CoreLib.Values;
using NaughtyAttributes;

namespace Game
{
    public class GamePrefs : Core.Module<GamePrefs>
    {
        [Layer]
        public int _ignoreLayer;
        [Layer]
        public int _playerLayer;
        
        public GlobalListGo _platforms;
        public GvFloat      _platformSpeedMul;
        public GvFloat      _platformCooldownMul;
        
        // =======================================================================
        public override void Init()
        {
        }
    }
}