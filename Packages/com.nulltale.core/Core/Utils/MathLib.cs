using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    public static class MathLib
    {
        public const           float   GoldenRatio = 1.61803398875f;
        public const           float   PIHalf      = Mathf.PI * 0.5f;
        public const           float   PI2         = Mathf.PI * 2;
        public static readonly Vector3 V3Half      = new Vector3(0.5f, 0.5f, 0.5f);
        public static readonly Vector2 V2Half      = new Vector2(0.5f, 0.5f);

        // =======================================================================
        public static IEnumerable<Vector2Int> Line(Vector2Int from, Vector2Int to)
        {
            return Line(from.x, from.y, to.x, to.y);
        }
        
        public static IEnumerable<Vector2Int> Line(int x, int y, int x2, int y2)
        {
            var w   = x2 - x;
            var h   = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0)
                dx1             = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0)
                dy1             = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0)
                dx2             = -1;
            else if (w > 0) dx2 = 1;
            var longest  = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest  = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0)
                    dy2             = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }

            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++)
            {
                yield return new Vector2Int(x, y);
                
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x         += dx1;
                    y         += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public static float Fib(float baseFib, int iterations, float stepLimit = Single.MaxValue)
        {
            var a = 0.0f;
            var b = baseFib;
            var c = 0.0f;
            for (var n = 0; n < iterations; n++)
            {
                c = Mathf.Min(a, stepLimit) + b;
                a = b;
                b = c;
            }

            return c;
        }

        public static void Fib(float baseFib, int iterations, List<float> values, float stepLimit = Single.MaxValue)
        {
            var a = 0.0f;
            var b = baseFib;
            values.Add(a);
            values.Add(b);
            for (var n = 0; n < iterations; n++)
            {
                var c = Mathf.Min(a, stepLimit) + b;
                a = b;
                b = c;
                values.Add(c);
            }
        }
    }
    }