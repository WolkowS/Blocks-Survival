using UnityEngine;

namespace CoreLib
{
    public sealed class Id : MonoBehaviour, IId
    {
        [SerializeField]
        private UniqueId		m_UniqueID;
	
        public string		Key
        {
            get => m_UniqueID.Key;
            set => m_UniqueID.Key = value;
        }

        // =======================================================================
        public void GenerateGuid()
        {
            m_UniqueID.GenerateGuid();
        }
    }
}