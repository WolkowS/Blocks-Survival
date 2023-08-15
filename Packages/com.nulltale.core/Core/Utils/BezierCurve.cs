using UnityEngine;

namespace CoreLib
{
    public struct BezierCurve
    {
        public Vector3 start;
        public Vector3 startTangent;
        public Vector3 endTangent;
        public Vector3 end;

        // =======================================================================
        public BezierCurve(Vector3 start, Vector3 startTangent, Vector3 endTangent, Vector3 end)
        {
            this.start        = start;
            this.startTangent = startTangent;
            this.endTangent   = endTangent;
            this.end          = end;
        }

        public Vector3 ClosestPoint(Vector3 point, out float t, float sqrError = 0.001f)
        {
            var closest = BezierUtility.ClosestPointOnCurve(
                point,
                start, end, startTangent, endTangent,
                out t, sqrError);

            return closest;
        }

        public Vector3 PointAt(float t)
        {
            return BezierUtility.BezierPoint(
                start, startTangent, endTangent, end, t);
        }

        public Vector3 TangentAt(float t)
        {
            return BezierUtility.BezierTangent(
                start, startTangent, endTangent, end, t);
        }
    }
    
    public static class BezierUtility
    {
        static Vector3[] s_TempPoints = new Vector3[3];

        // =======================================================================
        public static Vector3 BezierPoint(Vector3 startPosition, Vector3 startTangent, Vector3 endTangent, Vector3 endPosition, float t)
        {
            var s = 1.0f - t;
            return startPosition * s * s * s + startTangent * s * s * t * 3.0f + endTangent * s * t * t * 3.0f + endPosition * t * t * t;
        }

        public static Vector3 BezierTangent(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            var C1 = (d - (3.0f * c) + (3.0f * b) - a);
            var C2 = ((3.0f * c) - (6.0f * b) + (3.0f * a));
            var C3 = ((3.0f * b) - (3.0f * a));
            //Vector3 C4 = (a);

            return (3.0f * C1 * t * t) + (2.0f * C2 * t) + C3;
        }

        public static Vector3 ClosestPointOnCurve(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, out float t, float sqrError = 0.001f)
        {
            var startToEnd = endPosition - startPosition;
            var startToTangent = (startTangent - startPosition);
            var endToTangent = (endTangent - endPosition);

            if (Colinear(startToTangent, startToEnd, sqrError) && Colinear(endToTangent, startToEnd, sqrError))
                return ClosestPointToSegment(point, startPosition, endPosition, out t);

            var leftStartT = 0f;
            var leftEndT = 0.5f;
            var rightStartT = 0.5f;
            var rightEndT = 1f;

            SplitBezier(0.5f, startPosition, endPosition, startTangent, endTangent,
                out var leftStartPosition, out var leftEndPosition, out var leftStartTangent, out var leftEndTangent,
                out var rightStartPosition, out var rightEndPosition, out var rightStartTangent, out var rightEndTangent);

            var pointLeft = ClosestPointOnCurveIterative(point, leftStartPosition, leftEndPosition, leftStartTangent, leftEndTangent, sqrError, ref leftStartT, ref leftEndT);
            var pointRight = ClosestPointOnCurveIterative(point, rightStartPosition, rightEndPosition, rightStartTangent, rightEndTangent, sqrError, ref rightStartT, ref rightEndT);

            if ((point - pointLeft).sqrMagnitude < (point - pointRight).sqrMagnitude)
            {
                t = leftStartT;
                return pointLeft;
            }

            t = rightStartT;
            return pointRight;
        }

        public static Vector3 ClosestPointOnCurveFast(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, out float t)
        {
            var sqrError = 0.001f;
            var startT = 0f;
            var endT = 1f;

            var closestPoint = ClosestPointOnCurveIterative(point, startPosition, endPosition, startTangent, endTangent, sqrError, ref startT, ref endT);

            t = startT;

            return closestPoint;
        }

        private static Vector3 ClosestPointOnCurveIterative(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float sqrError, ref float startT, ref float endT)
        {
            while ((startPosition - endPosition).sqrMagnitude > sqrError)
            {
                var startToEnd = endPosition - startPosition;
                var startToTangent = (startTangent - startPosition);
                var endToTangent = (endTangent - endPosition);

                if (Colinear(startToTangent, startToEnd, sqrError) && Colinear(endToTangent, startToEnd, sqrError))
                {
                    var closestPoint = ClosestPointToSegment(point, startPosition, endPosition, out var t);
                    t *= (endT - startT);
                    startT += t;
                    endT -= t;
                    return closestPoint;
                }

                SplitBezier(0.5f, startPosition, endPosition, startTangent, endTangent,
                            out var leftStartPosition, out var leftEndPosition, out var leftStartTangent, out var leftEndTangent,
                            out var rightStartPosition, out var rightEndPosition, out var rightStartTangent, out var rightEndTangent);

                s_TempPoints[0] = leftStartPosition;
                s_TempPoints[1] = leftStartTangent;
                s_TempPoints[2] = leftEndTangent;

                var sqrDistanceLeft = SqrDistanceToPolyLine(point, s_TempPoints);

                s_TempPoints[0] = rightEndPosition;
                s_TempPoints[1] = rightEndTangent;
                s_TempPoints[2] = rightStartTangent;

                var sqrDistanceRight = SqrDistanceToPolyLine(point, s_TempPoints);

                if (sqrDistanceLeft < sqrDistanceRight)
                {
                    startPosition = leftStartPosition;
                    endPosition = leftEndPosition;
                    startTangent = leftStartTangent;
                    endTangent = leftEndTangent;

                    endT -= (endT - startT) * 0.5f;
                }
                else
                {
                    startPosition = rightStartPosition;
                    endPosition = rightEndPosition;
                    startTangent = rightStartTangent;
                    endTangent = rightEndTangent;

                    startT += (endT - startT) * 0.5f;
                }
            }

            return endPosition;
        }

        public static void SplitBezier(float t, Vector3 startPosition, Vector3 endPosition, Vector3 startRightTangent, Vector3 endLeftTangent,
            out Vector3 leftStartPosition, out Vector3 leftEndPosition, out Vector3 leftStartTangent, out Vector3 leftEndTangent,
            out Vector3 rightStartPosition, out Vector3 rightEndPosition, out Vector3 rightStartTangent, out Vector3 rightEndTangent)
        {
            var tangent0 = (startRightTangent - startPosition);
            var tangent1 = (endLeftTangent - endPosition);
            var tangentEdge = (endLeftTangent - startRightTangent);

            var tangentPoint0 = startPosition + tangent0 * t;
            var tangentPoint1 = endPosition + tangent1 * (1f - t);
            var tangentEdgePoint = startRightTangent + tangentEdge * t;

            var newTangent0 = tangentPoint0 + (tangentEdgePoint - tangentPoint0) * t;
            var newTangent1 = tangentPoint1 + (tangentEdgePoint - tangentPoint1) * (1f - t);
            var newTangentEdge = newTangent1 - newTangent0;

            var bezierPoint = newTangent0 + newTangentEdge * t;

            leftStartPosition = startPosition;
            leftEndPosition = bezierPoint;
            leftStartTangent = tangentPoint0;
            leftEndTangent = newTangent0;

            rightStartPosition = bezierPoint;
            rightEndPosition = endPosition;
            rightStartTangent = newTangent1;
            rightEndTangent = tangentPoint1;
        }

        private static Vector3 ClosestPointToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, out float t)
        {
            var relativePoint = point - segmentStart;
            var segment = (segmentEnd - segmentStart);
            var segmentDirection = segment.normalized;
            var length = segment.magnitude;

            var dot = Vector3.Dot(relativePoint, segmentDirection);

            if (dot <= 0f)
                dot = 0f;
            else if (dot >= length)
                dot = length;

            t = dot / length;

            return segmentStart + segment * t;
        }

        private static float SqrDistanceToPolyLine(Vector3 point, Vector3[] points)
        {
            var minDistance = float.MaxValue;

            for (var i = 0; i < points.Length - 1; ++i)
            {
                var distance = SqrDistanceToSegment(point, points[i], points[i + 1]);

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        private static float SqrDistanceToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            var relativePoint = point - segmentStart;
            var segment = (segmentEnd - segmentStart);
            var segmentDirection = segment.normalized;
            var length = segment.magnitude;

            var dot = Vector3.Dot(relativePoint, segmentDirection);

            if (dot <= 0f)
                return (point - segmentStart).sqrMagnitude;
            else if (dot >= length)
                return (point - segmentEnd).sqrMagnitude;

            return Vector3.Cross(relativePoint, segmentDirection).sqrMagnitude;
        }

        private static bool Colinear(Vector3 v1, Vector3 v2, float error = 0.0001f)
        {
            return Mathf.Abs(v1.x * v2.y - v1.y * v2.x + v1.x * v2.z - v1.z * v2.x + v1.y * v2.z - v1.z * v2.y) < error;
        }
    }
}