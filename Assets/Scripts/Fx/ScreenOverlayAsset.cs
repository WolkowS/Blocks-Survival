using CoreLib;
using CoreLib.Module;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class ScreenOverlayAsset : ScriptableObject
    {
        public Color          _color;
        public AnimationCurve _alpha;

        public Optional<int>  _sortingOrder;
        
        // =======================================================================
        public enum Mode
        {
            Flash
        }
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            FxTools.CreateScreenOverlayEffect(_color, _alpha, sortingOreder: _sortingOrder.GetValueOrDefault(10000));
        }
    }
}