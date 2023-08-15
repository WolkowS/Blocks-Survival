using UnityEngine;

namespace CoreLib.Values
{
    public class GvColor : GlobalValue<Color>
    {
        public override string ToString()
        {
            return Value.ToString();
        }
        
        internal override string Serialize()
        {
            return JsonUtility.ToJson(Value, false);
        }

        internal override void Deserialize(string data)
        {
            Value = JsonUtility.FromJson<Color>(data);
        }
    }
}