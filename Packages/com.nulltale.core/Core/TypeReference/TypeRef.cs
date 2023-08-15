using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TypeRef : ISerializationCallbackReceiver
    {
        private Type		m_Type;
        [SerializeField]
        private string		m_TypeName;
        [SerializeField]
        private string		m_GUID;

        public Type Type
        {
            get => m_Type;
            set
            {
                m_Type     = value;
                m_TypeName = m_Type != null ? m_Type.AssemblyQualifiedName : string.Empty;
                m_GUID = m_Type?.GUID.ToString();
            }
        }

        // =======================================================================
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (m_TypeName.IsNullOrEmpty())
            {
                m_GUID = string.Empty;
                return;
            }
            
            var type = Type.GetType(m_TypeName); 
            if (type == null)
            {
                if (m_GUID.IsNullOrEmpty() == false)
                { 
                    // fix type name
                    var guid  = new Guid(m_GUID);
                    var found = UnityEditor.TypeCache.GetTypesWithAttribute<GuidAttribute>().FirstOrDefault(n => n.GUID == guid);
                    if (found != null)
                        m_TypeName = found.AssemblyQualifiedName;
                }
            }
            else
            {
                m_GUID = type.GUID.ToString();
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(m_TypeName) == false) 
            {
                m_Type = Type.GetType(m_TypeName);

                if (m_Type == null)
                {
                    Debug.LogWarning($"'{m_TypeName}' was referenced but class type was not found. Guid ({m_GUID})");
                }
            }
            else 
            {
                m_Type = null;
            }
        }

        protected bool Equals(TypeRef other)
        {
            return m_TypeName == other.m_TypeName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var objType = obj.GetType();
            if (objType == this.GetType())
                return Equals((TypeRef)obj);
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