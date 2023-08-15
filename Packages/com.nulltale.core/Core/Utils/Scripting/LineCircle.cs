using System;
using Unity.Collections;
using UnityEngine;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    public class LineCircle : MonoBehaviour
    {
        private const  float                k_2PI    = Mathf.PI * 2f;
        private static NativeArray<Vector3> s_Buffer = new NativeArray<Vector3>(256, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        private LineRenderer _line;
        public  int          _segments = 3;
        public  float        _thickness;
        
        // =======================================================================
        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (_line.positionCount != _segments)
            {
                // radius has same effect as scale
                for (var index = 0; index < _segments; index++)
                {
                    var angleRad = (index / (float)_segments) * k_2PI;
                    s_Buffer[index] = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);
                }

                _line.positionCount = _segments;
                _line.SetPositions(s_Buffer);
            }

            if (_line.widthMultiplier != _thickness)
                _line.widthMultiplier = _thickness;
        }
    }
}