using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeSerializableDictionary
{
    /// <summary>
    /// This SerializableKVP stores the Key and the Value(s) for the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/>.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks>
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableKVP<K, V>
    {
        [SerializeField]
        private K _key;
        public K Key => _key;

        [SerializeField]
        private V _value;
        public V Value => _value;

        public SerializableKVP(K key, V value)
        {
            _key = key;
            _value = value;
        }
        
        public static implicit operator V(SerializableKVP<K, V> val)
        {
            return val._value;
        }
    }

    /// <summary>
    /// This SerializableKVP stores the Key and the Value(s) for the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/>.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks>
    /// This alternate version boxes the values into a <seealso cref="System.Collections.Generic.List{T}"/>
    /// so we can bind the data to the List and effectively serialize types like
    /// (see: <seealso cref="UnityEngine.Vector3"/>).
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableKVPBoxed<K, V> : List<V>
    {
        [SerializeField] private K       _key;
        public                   K       Key => _key;
        [SerializeField] private List<V> _values;
        public                   List<V> Values => _values;

        public V Value => _values[0];

        public static implicit operator V(SerializableKVPBoxed<K, V> val)
        {
            return val.Value;
        }

        public SerializableKVPBoxed(K key, List<V> values)
        {
            _key = key;
            _values = values;
        }
        public SerializableKVPBoxed(K key, V value)
        {
            _key = key;
            List<V> container = new List<V>();
            container.Add(value);
            _values = container;
        }
    }
}