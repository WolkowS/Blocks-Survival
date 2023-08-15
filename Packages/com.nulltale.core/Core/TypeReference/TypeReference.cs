using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        private Type		m_Type;
        [SerializeField]
        private string		m_TypeName;

        public Type Type
        {
            get => m_Type;
            set
            {
                m_Type     = value;
                m_TypeName = m_Type != null ? m_Type.AssemblyQualifiedName : String.Empty;;
            }
        }

        // =======================================================================
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(m_TypeName) == false) 
            {
                m_Type = Type.GetType(m_TypeName);

                if (m_Type == null)
                    Debug.LogWarning($"'{m_TypeName}' was referenced but class type was not found.");
            }
            else 
            {
                m_Type = null;
            }
        }

        protected bool Equals(TypeReference other)
        {
            return m_TypeName == other.m_TypeName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var objType = obj.GetType();
            if (objType == this.GetType())
                return Equals((TypeReference)obj);
            if (objType == typeof(Type))
                return Equals(m_Type, obj);

            return false;

        }

        public override int GetHashCode()
        {
            return (m_TypeName != null ? m_TypeName.GetHashCode() : 0);
        }
    }
}