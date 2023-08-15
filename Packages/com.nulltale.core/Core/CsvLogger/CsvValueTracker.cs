using System;
using System.Collections.Generic;
using System.Reflection;
using CoreLib.Values;

namespace CoreLib.CSVLogger
{
    public class CsvValueTracker : CoreLib.Module.CsvLogger.Module
    {
        public GlobalValue      _value;
        
        // =======================================================================
        public override string Value()
        {
            return _value.Serialize();
        }

        public override void Init(Module.CsvLogger logger)
        {
            logger._postRequest = true;
        }
    }
}