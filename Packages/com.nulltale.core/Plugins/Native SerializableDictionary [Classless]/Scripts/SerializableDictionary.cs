using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeSerializableDictionary
{
    /// <summary>
    /// This SerializableDictionary works like a regular <seealso cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
    /// It does not require a new class for every data type.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks> 
    /// It can be viewed within the Unity Inspector Window (Editor).
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, SerializableKVP<K, V>>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableKVP<K, V>> _keys = new List<SerializableKVP<K, V>>();

        // This is just a niceity which allows for a dictionary to become a SerializableDictionary directly
        public static implicit operator SerializableDictionary<K, V>(Dictionary<K, V> dictionary)
        {
            SerializableDictionary<K, V> serializableDict = dictionary;
            return serializableDict;
        }

        /// <summary>
        /// Adds directly to the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/> without needing to manually encapsulate the value in a <seealso cref="NativeSerializableDictionary.SerializableKVP{K, V}"/> container.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void AddDirect(K key, V value)
        {
            Add(key, new SerializableKVP<K, V>(key, value));
        }

        /// <summary>
        /// Attempts to retrieve a value from a key, passing out the ref to the value provided.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/> with.</param>
        /// <param name="value">The desired value to retrieve. If it returns false, the value returned will be null.</param>
        /// <returns>The bool will return true if it successfully retrieves the value from the key provided.</returns>
        public bool TryGetValue(K key, ref V value)
        {
            SerializableKVP<K, V> defaultValue;
            bool canGet = TryGetValue(key, out defaultValue);
            if (defaultValue != null)
                value = defaultValue.Value;
            return canGet;
        }

        /// <summary>
        /// Attempts to retrieve a value from a key.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/> with.</param>
        /// <returns>If the key doesn't exist or the value is null a NullReferenceException will be raised.</returns>
        public V GetValue(K key)
        {
            SerializableKVP<K, V> defaultValue;
            TryGetValue(key, out defaultValue);
            return defaultValue.Value;
        }

        public void OnBeforeSerialize()
        {
            // This protects us from having
            // entries constantly added.
            if (!(Count > _keys.Count)) return;
            foreach (var kvp in this)
            {
                _keys.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var kvp in _keys)
            {
                if (kvp == null) continue;
                if (kvp.Key == null) continue;
                if (kvp.Value == null) continue;
                // This allows us to wait for duplicates
                // to be changed before adding them
                if (ContainsKey(kvp.Key)) continue;

                Add(kvp.Key, kvp);
            }
        }
    }
    
    /// <summary>
    /// This SerializableDictionary works like a regular <seealso cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
    /// It does not require a new class for every data type.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks> 
    /// This alternate version boxes the values into a <seealso cref="System.Collections.Generic.List{T}"/>
    /// in the internal referenced custom class <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/>
    /// so we can bind the data to the List and effectively serialize types like
    /// (see: <seealso cref="UnityEngine.Vector3"/>).
    /// It can be viewed within the Unity Inspector Window (Editor).
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableDictionaryBoxed<K, V> : Dictionary<K, SerializableKVPBoxed<K, V>>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableKVPBoxed<K, V>> _keys = new List<SerializableKVPBoxed<K, V>>();

        // This is just a niceity which allows for a dictionary to become a SerializableDictionary directly
        public static implicit operator SerializableDictionaryBoxed<K, V>(Dictionary<K, V> dictionary)
        {
            SerializableDictionaryBoxed<K, V> serializableDict = dictionary;
            return serializableDict;
        }

        /// <summary>
        /// Adds directly to the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> without needing to manually encapsulate the value in a <seealso cref="NativeSerializableDictionary.SerializableKVPBoxed{K, V}"/> container.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void AddDirect(K key, V value)
        {
            Add(key, new SerializableKVPBoxed<K, V>(key, value));
        }
        /// <summary>
        /// Adds directly to the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> without needing to manually encapsulate the value in a <seealso cref="NativeSerializableDictionary.SerializableKVPBoxed{K, V}"/> container.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void AddDirect(K key, List<V> value)
        {
            Add(key, new SerializableKVPBoxed<K, V>(key, value));
        }

        /// <summary>
        /// Attempts to retrieve an index 0 direct value from a key, passing out the ref to the value provided.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> with.</param>
        /// <param name="value">The desired value to retrieve. If it returns false, the value returned will be null.</param>
        /// <returns>The bool will return true if it successfully retrieves the value from the key provided.</returns>
        public bool TryGetValue(K key, ref V value)
        {
            SerializableKVPBoxed<K, V> defaultValue;
            bool canGet = TryGetValue(key, out defaultValue);
            if (defaultValue != null)
                value = defaultValue.Value;
            return canGet;
        }

        /// <summary>
        /// Attempts to retrieve an index 0 direct value from a key.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> with.</param>
        /// <returns>If the key doesn't exist or the value is null a NullReferenceException will be raised.</returns>
        public V GetValue(K key)
        {
            SerializableKVPBoxed<K, V> defaultValue;
            TryGetValue(key, out defaultValue);
            return defaultValue.Value;
        }

        /// <summary>
        /// Attempts to retrieve a boxed value from a key, passing out the ref to the value provided.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> with.</param>
        /// <param name="value">The desired value to retrieve. If it returns false, the value returned will be null.</param>
        /// <returns>The bool will return true if it successfully retrieves the value from the key provided.</returns>
        public bool TryGetValues(K key, ref List<V> values)
        {
            SerializableKVPBoxed<K, V> defaultValue;
            bool canGet = TryGetValue(key, out defaultValue);
            if (defaultValue != null)
                values = defaultValue.Values;
            return canGet;
        }

        /// <summary>
        /// Attempts to retrieve a boxed value from a key.
        /// </summary>
        /// <param name="key">The key to search in the <seealso cref="NativeSerializableDictionary.SerializableDictionaryBoxed{K, V}"/> with.</param>
        /// <returns>If the key doesn't exist or the value is null a NullReferenceException will be raised.</returns>
        public List<V> GetValues(K key)
        {
            SerializableKVPBoxed<K, V> defaultValue;
            TryGetValue(key, out defaultValue);
            return defaultValue.Values;
        }

        public void OnBeforeSerialize()
        {
            // This protects us from having
            // entries constantly added.
            if (!(Count > _keys.Count)) return;
            foreach (var kvp in this)
            {
                _keys.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var kvp in _keys)
            {
                if (kvp == null) continue;
                if (kvp.Key == null) continue;
                if (kvp.Values == null) continue;

                // This allows us to wait for duplicates
                // to be changed before adding them
                if (ContainsKey(kvp.Key)) continue;

                Add(kvp.Key, kvp);

                // we set the range of the class' inherited List
                // and then we copy over the contents, making sure
                // that the internal List is converted first so it
                // can use the CopyTo function to bind the data
                kvp.AddRange(kvp.Values);
                kvp.Values.CopyTo(kvp.ToArray());
            }
        }
    }
}
