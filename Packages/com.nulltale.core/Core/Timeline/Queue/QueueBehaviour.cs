using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class QueueBehaviour : PlayableBehaviour
    {
        private static Dictionary<IdAsset, Queue<QueueBehaviour>> s_Queues = new Dictionary<IdAsset, Queue<QueueBehaviour>>();

        public  IdAsset               m_QueueId;
        private Queue<QueueBehaviour> m_Queue;
        public  TimelineClip          m_Clip;

        public PlayableDirector m_Director;

        private bool m_IsPlaing;
        private bool m_Hold;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (m_IsPlaing)
                return;

            m_IsPlaing = true;
            if (s_Queues.TryGetValue(m_QueueId, out m_Queue) == false)
                s_Queues.Add(m_QueueId, m_Queue = new Queue<QueueBehaviour>());

            m_Queue.Enqueue(this);

            PrepareFrame(playable, info);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (m_IsPlaing == false)
                return;

            if (m_Director == null)
                return;

            var hold = m_Queue.Peek() != this;
            if (m_Hold == hold)
                return;

            m_Hold = hold;
            playable.GetGraph().GetRootPlayable(0).SetSpeed(m_Hold ? 0d : 1d);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_Director == null)
                return;

            if (m_IsPlaing == false)
                return;

            if (m_Hold)
            {
                m_Director.time = m_Clip.end - ControlBehaviour.s_TimeHorizon;
                return;
            }

            m_IsPlaing = false;
            m_Queue.Dequeue();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void _domainReloadCapability()
        {
            s_Queues = new Dictionary<IdAsset, Queue<QueueBehaviour>>();
        }
    }
}