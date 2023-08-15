using UnityEngine.Playables;

namespace CoreLib.Tween
{
    public class TweenDirector : Tween
    {
        private PlayableDirector m_Director;
        private float            m_Impact;
        
        // =======================================================================
        private void Awake()
        {
            m_Director = m_Root.GetValueOrDefault(gameObject).GetComponent<PlayableDirector>();
        }

        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

            m_Director.time += impact - m_Impact;
            m_Director.Evaluate();
            m_Impact        =  impact;
        }

        public override void Revert()
        {
            m_Director.time -= m_Impact;
            m_Director.Evaluate();
            m_Impact        =  0;
        }
    }
}