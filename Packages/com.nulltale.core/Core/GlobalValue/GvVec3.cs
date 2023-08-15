using UnityEngine;

namespace CoreLib.Values
{
    public class GvVec3 : GlobalValue<Vector3>
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
            Value = JsonUtility.FromJson<Vector3>(data);
        }
    }
}