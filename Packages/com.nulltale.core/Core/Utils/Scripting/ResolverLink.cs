using System;

namespace CoreLib.Scripting
{
    [Serializable]
    public class ResolverLink<T> : IRefGet<T>, IRefSet<T>
    {
        public  Resolver   _resolver;
        public  string     _path;
        private IRefGet<T> _linkGet;
        private IRefSet<T> _linkSet;
        private bool       _resolved;

        public T Value
        {
            set
            {
                _resolve();
                _linkSet.Value = value;
            }
            
            get
            {
                _resolve();
                return _linkGet.Value;
            }
        }

        // =======================================================================
        public void Resolve() => _resolved = false;
        
        protected void _resolve()
        {
            if (_resolved)
                return;
            
            var refField = _resolver.GetRef<T>(_path);

            _linkGet = refField;
            _linkSet = refField;

            _resolved = true;
        }
    }
}