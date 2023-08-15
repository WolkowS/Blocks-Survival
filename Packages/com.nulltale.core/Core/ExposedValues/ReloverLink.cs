using UnityEngine;

namespace CoreLib.ExposedValues
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class ReloverLink : MonoBehaviour, IResolver
    {
        public Object    m_Source;
        public IResolver m_Resolver;

        // =======================================================================
        private void Awake()
        {
            if (m_Source == null)
                m_Source = (Object)GetComponentInParent<IResolver>();

            m_Resolver = (IResolver)m_Source;
        }

        public object Resolve(PropertyName id)
        {
            return m_Resolver?.Resolve(id);
        }
    }
}