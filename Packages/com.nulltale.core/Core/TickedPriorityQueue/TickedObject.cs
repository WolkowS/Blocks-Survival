using System;

namespace CoreLib
{
	public sealed class TickedObject : ITicked
	{
        public event Action Tick;

        float ITicked.TickLength { get => m_TickedLenght; }

        private TickedQueue m_TickedQueue;
        private bool        m_Loop;
        private float       m_TickedLenght;

        // =======================================================================
		public TickedObject(Action callback, TickedQueue tickedQueue, float tickLength, bool loop)
		{
            Tick           = callback;
            m_TickedLenght = tickLength;
            m_TickedQueue  = tickedQueue;
            m_Loop         = loop;
		}

        public void Enqueue()
        {
            m_TickedQueue.Add(this, m_Loop);
        }

        public void Dequeue()
        {
            m_TickedQueue.Remove(this);
        }

        void ITicked.OnTicked()
        {
            Tick?.Invoke();
        }
	}
}

