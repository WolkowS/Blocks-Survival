using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.ExposedValues
{
    public abstract class ExposedValue : MonoBehaviour
    {
        public Object m_Owner;
        public string m_ID;

        public string Path => $"{m_Owner.GetType().Name}.{m_ID}";
    }
    
    public abstract class ExposedValue<T> : ExposedValue, IRefGet<T>, IRefSet<T>
    {
        public T m_Value;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public bool HasValue => true;
    }
    
    public abstract class ExposedCondition : MonoBehaviour
    {
        public abstract bool IsMet();
    }

    public interface IResolver
    {
        public object Resolve(PropertyName id);
    }

    internal interface IResolvable
    {
    }
}