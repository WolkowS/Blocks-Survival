using System.Globalization;

namespace CoreLib.CSVLogger
{
    public class CsvPostStamp : CoreLib.Module.CsvLogger.Module
    {
        private float                    _time;
        private bool                     _hasValue;
        private CoreLib.Module.CsvLogger _logger;

        // =======================================================================
        public void Post()
        {
            _time                = _logger._time;
            _hasValue            = true;
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
            return _time.ToString("F1", CultureInfo.InvariantCulture);
        }

        public override string Name()
        {
            return _name.GetValueOrDefault(name);
        }
    }
}