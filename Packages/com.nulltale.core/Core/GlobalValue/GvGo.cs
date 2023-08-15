using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.Values
{
    public class GvGo : GlobalValue<GameObject>
    {
        public void SetActive(bool active)
        {
            Value.SetActive(active);
        }
        
        public void InvokeSignal(SignalAsset signal)
        {
            Value.gameObject.InvokeSignal(signal);
        }

        public void Clear()
        {
            Value = null;
        }
        
        public void Destroy()
        {
            Destroy(Value);
        }
        
        public void SetParent(GameObject parent)
        {
            Value.transform.SetParent(parent.transform);
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