using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class CanvasGroupMixerBehaviour : PlayableBehaviour
    {
        private float   _alphaInitial;
        private bool    _blockRaycastInitial;
        private bool    _interactableInitial;

        private CanvasGroup _canvas;

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_canvas != null)
            {
                _canvas.alpha          = _alphaInitial;
                _canvas.blocksRaycasts = _blockRaycastInitial;
                _canvas.interactable   = _interactableInitial;

                _canvas = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_canvas == null)
            {
                _canvas = (CanvasGroup)playerData;
                
                _alphaInitial        = _canvas.alpha;
                _blockRaycastInitial = _canvas.blocksRaycasts;
                _interactableInitial = _canvas.interactable;
            }
            
            var inputCount = playable.GetInputCount();

            // calculate weights
            var alpha = 0f;
            var color = Color.clear;
            
            var interactableWeight  = 0f;
            var blockRaycastsWeight = 0f;
            
            var blockRaycasts = false;
            var interactable  = false;

            var fullWeight = 0f;
            var soloInput = 0;

            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                if (inputWeight <= 0f)
                    continue;

                soloInput  =  n;
                fullWeight += inputWeight;

                var inputPlayable = (ScriptPlayable<CanvasGroupBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                alpha += behaviour.m_Alpha * inputWeight;

                // weighted impact of optional values
                if (behaviour.m_BlockRaycasts.Enabled && blockRaycastsWeight < inputWeight)
                {
                    blockRaycastsWeight = inputWeight;
                    blockRaycasts = behaviour.m_BlockRaycasts.Value;
                }
                
                if (behaviour.m_Interactable.Enabled && interactableWeight < inputWeight)
                {
                    interactableWeight = inputWeight;
                    interactable = behaviour.m_BlockRaycasts.Value;
                }
            }

            _canvas.alpha  = alpha;
            
            _canvas.interactable   = interactableWeight > 0f ? blockRaycasts : _interactableInitial;
            _canvas.blocksRaycasts = blockRaycastsWeight > 0f ? blockRaycasts : _blockRaycastInitial;
        }
    }
}