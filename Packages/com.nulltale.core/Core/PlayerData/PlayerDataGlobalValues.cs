using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataGlobalValues : PlayerData.DataHandle
    {
        public List<GlobalValue> m_Values;

        // =======================================================================
        [Serializable]
        internal class SerializationData
        {
            public List<ValueData> Data;

            [Serializable]
            public struct ValueData
            {
                public string Name;
                public string Data;
            }
        }

        // =======================================================================
        public override void InitDefault()
        {
            foreach (var globalValue in m_Values)
                globalValue.Init();
        }

        public override void Init(string data)
        {
            // read values, name/data
            var valuesData = JsonUtility.FromJson<SerializationData>(data).Data;
            var values = m_Values.ToDictionary(n => n.name, n => n);
            
            foreach (var valueData in valuesData)
            {
                if (values.TryGetValue(valueData.Name, out var globalValue))
                    values.Remove(valueData.Name);

                if (globalValue == null)
                {
                    Debug.LogWarning($"\"{valueData.Name}\" not presented in data values");
                    continue;
                }

                try
                {
                    globalValue.Deserialize(valueData.Data);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            // not presented init to default
            foreach (var globalValue in values.Values)
                globalValue.Init();
        }

        public override string Serialize()
        {
            var serializationData = new SerializationData();
            serializationData.Data = m_Values.Where(n => n.IsNull() == false).Select(n => new SerializationData.ValueData { Name = n.name, Data = n.Serialize() }).ToList();
            return JsonUtility.ToJson(serializationData);
        }
    }
}