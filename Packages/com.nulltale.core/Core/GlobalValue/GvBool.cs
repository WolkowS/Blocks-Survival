using UnityEngine;

namespace CoreLib.Values
{
    public class GvBool : GlobalValue<bool>
    {
        public void Set(MonoBehaviour val)
        {
            Value = ((IRefGet<bool>)val).Value;
        }
        
        internal override string Serialize()
        {
            return Value.ToString();
        }

        internal override void Deserialize(string data)
        {
            Value = bool.Parse(data);
        }
    }
}