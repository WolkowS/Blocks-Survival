using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ValueMixerBehaviour : PlayableBehaviour
    {
        private IPlayableValue       m_PlayableTarget;
        private IPlayableValueHandle m_PlayableValueHandle;
        public  SetMode              m_Mode;
        public  Ref<float>           m_Blend;
        public  int                  m_Priority;

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_PlayableValueHandle != null)
            {
                m_PlayableTarget.UnLock(m_PlayableValueHandle);

                m_PlayableTarget      = null;
                m_PlayableValueHandle = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var target = playerData as IPlayableValue;
            if (m_PlayableTarget != target)
            {
                // release handle
                if (m_PlayableValueHandle != null)
                {
                    m_PlayableTarget?.UnLock(m_PlayableValueHandle);
                    m_PlayableValueHandle = null;
                }

                m_PlayableTarget = target;

                // open handle
                if (m_PlayableTarget != null)
                {
                    m_PlayableValueHandle = m_PlayableTarget.Lock();
                    if (m_PlayableValueHandle is IPlayableValueHandleOptions handleOptions)
                        handleOptions.Init(m_Mode, m_Blend, m_Priority);
                }
            }

            if (m_PlayableValueHandle == null)
                return;

            var inputCount = playable.GetInputCount();

            // calculate weights
            var weight = 0f;
            var value  = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight = playable.GetInputWeight(n);
                var input = playable.GetInput(n);
                
                if (input.GetPlayableType() == typeof(ValueBehaviour))
                {
                    var behaviour = ((ScriptPlayable<ValueBehaviour>)input).GetBehaviour();
                    value += behaviour.m_Value * inputWeight;
                }
                else
                if (input.GetPlayableType() == typeof(ProgressBehaviour))
                {
                    var behaviour = ((ScriptPlayable<ProgressBehaviour>)input).GetBehaviour();
                    value += behaviour.Value * inputWeight;
                }
                
                weight += inputWeight;
            }

            m_PlayableValueHandle.Set(weight, weight >= 1f ? value : (value / (weight <= 0f ? 1f : weight)));
        }
    }
}