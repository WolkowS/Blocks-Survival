using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class UniqueId : IId
    {
        [SerializeField]
        private string			m_GUID;

        public string			Key
        {
            get => m_GUID;
            set => m_GUID = value;
        }

        // =======================================================================
        public UniqueId()
        {
            m_GUID = Guid.NewGuid().ToString();
        }

        public UniqueId(string id)
        {
            m_GUID = id;
        }

        public void GenerateGuid()
        {
            m_GUID = Guid.NewGuid().ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is UniqueId uid)
                return Equals(m_GUID, uid.m_GUID);
            if (obj is string str)
                return string.Equals(str, m_GUID);

            return false;
        }

        public override int GetHashCode()
        {
            return m_GUID.GetHashCode();
        }

        public override string ToString()
        {
            return m_GUID;
        }

        public static implicit operator string(UniqueId uid)
        {
            return uid.m_GUID;
        }
    }
}

public interface IId
{
	string Key { get; }
}