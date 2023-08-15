using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [Serializable]
    public class RefLink<T> : IRefGet<T>, IRefSet<T>
    {
        private static Dictionary<string, FieldInfo>    s_LinkCache = new Dictionary<string, FieldInfo>(128);
        
        // =======================================================================
        [SerializeField]
        protected Object _obj;
        [SerializeField]
        private string   _path;

        protected bool       _resolved;
        private   IRefGet<T> _linkGet;
        private   IRefSet<T> _linkSet;
        

        public T Value
        {
            set
            {
                _resolve();
                _linkSet.Value = value;
            }
            get
            {
#if UNITY_EDITOR
                if (_linkGet == null)
                    _resolved = false;
#endif
                _resolve();
                return _linkGet.Value;
            }
        }
        
        public void Resolve() => _resolved = false;

        // =======================================================================
        protected void _resolve()
        {
            if (_resolved)
                return;
            
            var refField = _resolveFrom(_obj);
            
            _linkGet = refField as IRefGet<T>;
            _linkSet = refField as IRefSet<T>;

            _resolved = true;
        }
        
        object _resolveFrom(Object obj)
        {
            if (obj is RefBus rb)
                return _resolveFrom(rb.Link);
            
            if (_path.IsNullOrEmpty())
                return obj;
         
            if (s_LinkCache.TryGetValue(_path, out var fieldInfo) == false)
            {
                fieldInfo = obj.GetType().GetField(_path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                s_LinkCache.Add(_path, fieldInfo);
            }
            
            return fieldInfo.GetValue(obj);
        }
    }
}