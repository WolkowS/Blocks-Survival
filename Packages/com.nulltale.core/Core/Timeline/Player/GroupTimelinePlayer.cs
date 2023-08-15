namespace CoreLib
{
    public sealed class GroupTimelinePlayer : TimelinePlayer
    {
        private ITimelinePlayer[] m_Handles;

        // =======================================================================
        private void Awake()
        {
            m_Handles = GetComponentsInChildren<ITimelinePlayer>();
        }

        public override void _play()
        {
            foreach (var handle in m_Handles)
                handle.Play();
        }

        public override void _stop()
        {
            foreach (var handle in m_Handles)
                handle.Stop();
        }
    }
}