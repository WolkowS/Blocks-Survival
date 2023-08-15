using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public struct Line
    {
        public Vector2 start;
        public Vector2 end;
        
        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end   = end;
        }
    }
}