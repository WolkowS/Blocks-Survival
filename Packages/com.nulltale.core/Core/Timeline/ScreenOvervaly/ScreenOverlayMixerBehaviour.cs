using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ScreenOverlayMixerBehaviour : PlayableBehaviour
    {
        public  int                                m_SortingOrder;
        private Module.FxTools.ScreenOverlayHandle m_OverlayHandle;

        private HashSet<ScreenOverlayBehaviour> _openSet  = new HashSet<ScreenOverlayBehaviour>();
        private HashSet<ScreenOverlayBehaviour> _closeSet = new HashSet<ScreenOverlayBehaviour>();

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_OverlayHandle = new Module.FxTools.ScreenOverlayHandle(m_SortingOrder);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_OverlayHandle?.Dispose();
            
            foreach (var sob in _closeSet)
                sob.ReleaseScreen();
            
            _closeSet.Clear();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            // calculate weights
            var scale       = 0f;
            var color       = Color.clear;
            var imageWeight = 0f;

            Sprite image = null;

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

                var inputPlayable = (ScriptPlayable<ScreenOverlayBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                if (inputWeight > 0f && behaviour.m_ScreenShot)
                    _openSet.Add(behaviour);
                
                scale += behaviour.m_Scale * inputWeight;
                color += behaviour.m_Color * inputWeight;

                if (imageWeight < inputWeight)
                {
                    image       = (behaviour.m_Image.Enabled || behaviour.m_ScreenShot) ? behaviour.m_Image.Value : null;
                    imageWeight = inputWeight;
                }
            }

            if (fullWeight > 0f)
                m_OverlayHandle.Open();
            else
            {
                m_OverlayHandle.Close();
                return;
            }

            // if single input, blend alpha, do nothing with scale
            if (fullWeight < 1f)
            {
                var behaviour = ((ScriptPlayable<ScreenOverlayBehaviour>)playable.GetInput(soloInput)).GetBehaviour();

                scale = behaviour.m_Scale;
                color = behaviour.m_Color.MulA(fullWeight);
            }

            foreach (var sob in _openSet)
            {
                if (_closeSet.Contains(sob) == false && sob.m_ScreenShot)
                    sob.TakeScreen();
            }
            
            foreach (var sob in _closeSet)
            {
                if (_openSet.Contains(sob) == false && sob.m_ScreenShot)
                    sob.ReleaseScreen();
            }
            
            _closeSet.Clear();
            _closeSet.UnionWith(_openSet);
            _openSet.Clear();
            
            m_OverlayHandle.Scale  = scale.ToVector2();
            m_OverlayHandle.Color  = color;
            m_OverlayHandle.Sprite = image;
        }
    }
}