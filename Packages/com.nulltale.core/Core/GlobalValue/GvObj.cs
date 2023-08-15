using UnityEngine;

namespace CoreLib.Values
{
    public class GvObj : GlobalValue<Object>
    {
        public T Get<T>() where T : class
        {
            return Value as T;
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }
        
        internal override string Serialize()
        {
            return string.Empty;
        }

        internal override void Deserialize(string data)
        {
        }
    }
}