using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.StaticInjector
{
    public class FieldInjector : ScriptableObject
    {
        public string _type;
        public string _assembly;
        public string _field;
        
        public int    _int;
        public float  _float;
        public string _string;
        public Object _obj;
        
        // =======================================================================
        public void Invoke()
        {
            var assembly = Assembly.Load(_assembly);
            var type     = assembly.GetType(_type);
            var field    = type.GetField(_field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogWarning($"Can't inject field {_field}", this);
                return;
            }
            
            if (field.FieldType == typeof(int)) 
                field.SetValue(null, _int);
            else if (field.FieldType == typeof(float))
                field.SetValue(null, _float);
            else if (field.FieldType == typeof(string))
                field.SetValue(null, _string);
            else if (field.FieldType.Implements<Object>())
                field.SetValue(null, _obj);
            else
                throw new ArgumentOutOfRangeException();
        }
    }
}