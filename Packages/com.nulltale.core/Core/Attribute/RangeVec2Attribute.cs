using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RangeVec2Attribute : PropertyAttribute
    {
        public float _xMin = float.NegativeInfinity;
        public float _xMax = float.PositiveInfinity;
        public float _yMin = float.NegativeInfinity;
        public float _yMax = float.PositiveInfinity;
        public bool     _lineal;
        
        // =======================================================================
        public RangeVec2Attribute(bool lineal = true)
        {
            _lineal = lineal;
        }
        
        public RangeVec2Attribute(float max, bool lineal = true) : this(lineal)
        {
            _xMin = 0;
            _xMax = max;
            _yMin = 0;
            _yMax = max;
        }
        
        public RangeVec2Attribute(float min, float max, bool lineal = true) : this(lineal)
        {
            _xMin = min;
            _xMax = max;
            _yMin = min;
            _yMax = max;
        }
        
        public RangeVec2Attribute(float xMin, float xMax, float yMin, float yMax, bool lineal = true) : this(lineal)
        {
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
        }
    }
}