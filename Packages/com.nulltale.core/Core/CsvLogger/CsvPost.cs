namespace CoreLib.CSVLogger
{
    public abstract class CsvPost<T> : CoreLib.Module.CsvLogger.Module
    {
        private T    _value;
        private bool _hasValue;
        private CoreLib.Module.CsvLogger _logger;

        // =======================================================================
        public void Post(T value)
        {
            _value    = value;
            _hasValue = true;
            _logger._postRequest = true;
        }

        public override void Init(Module.CsvLogger logger)
        {
            _logger = logger;
        }

        public override string Value()
        {
            if (_hasValue == false)
                return string.Empty;
            
            _hasValue = false;
            return _value.ToString();
        }

        public override string Name()
        {
            return _name.GetValueOrDefault(name);
        }
    }
}