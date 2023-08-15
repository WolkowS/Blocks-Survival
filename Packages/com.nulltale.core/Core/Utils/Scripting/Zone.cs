using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace CoreLib.Scripting
{
	public class Zone : MonoBehaviour
    {
        public Collider2D _collder;
        
        // =======================================================================
        public Vector2 RandomPoint()
        {
            switch(_collder)
            {
                case PolygonCollider2D col:
                    return _getRandomPoint(col);
                case CircleCollider2D col:
					return Random.insideUnitCircle.Mul(col.transform.lossyScale.To2DXY()) * col.radius;
                case BoxCollider2D col:
				{
					var point = col.size.Mul(new Vector2(Random.value, Random.value)) - col.size * .5f;
					point += col.offset;
					point =  point.Mul(col.transform.lossyScale.To2DXY());
					point = (col.transform.rotation * point.To3DXY()).To2DXY();
					point += col.transform.position.To2DXY();
					
					return point;
				}
                default:
                    throw new Exception();
            }
        }
        
        private static Vector2 _getRandomPoint(PolygonCollider2D polygonCollider2D)
        {
            var _triangles = Triangulator.Triangulate(polygonCollider2D.points.ToList());
            var triangle = _triangles[Random.Range(0, _triangles.Count)];
            var onePoint = Vector2.zero;
            var twoPoint = Vector2.zero;
            switch (Random.Range(0, 3))
            {
                case 0:
                    onePoint = triangle.a;
                    twoPoint = triangle.b;
                    break;
                case 1:
                    onePoint = triangle.b;
                    twoPoint = triangle.c;
                    break;
                case 2:
                    onePoint = triangle.a;
                    twoPoint = triangle.c;
                    break;
            }

            var center = triangle.centerOfMass();
            Vector2 randomBetween2Vector = _randomPointBetween2Points(onePoint, twoPoint);
            Vector2 randomPoint = _randomPointBetween2Points(center, randomBetween2Vector);
            return randomPoint;
        }

        private static Vector2 _randomPointBetween2Points(Vector2 start, Vector2 end)
        {
            return (start + Random.Range(0f, 1f) * (end - start));
        }

		public static class Triangulator
		{
			[Serializable] 
			public struct Triangle
			{
				public Vector2 a, b, c;

				/** Constructor with params for the three vertices of the triangle. */
				public Triangle(Vector2 _a, Vector2 _b, Vector2 _c)
				{
					a = _a;
					b = _b;
					c = _c;
				}

				public Vector2 centerOfMass()
				{
					return new Vector2((a.x + b.x + c.x) / 3, (a.y + b.y + c.y) / 3);
				}
			}

			[Serializable] 
			public struct Segment
			{
				public Vector2 a, b;

				/** Constructor with params for the two vertices of the segment. */
				public Segment(Vector2 _a, Vector2 _b)
				{
					a = _a;
					b = _b;
				}

				/** Utility to get the center point of the line segment. */
				public Vector2 Center
				{
					get { return (a + b) * 0.5f; }
				}
			}

			public static void RotatePathClockwise(ref List<Vector2> path)
			{
				if (path == null) throw new ArgumentNullException("Path must not be null!");
				if (path.Count < 1)
				{
					Debug.LogWarning("Attempting to rotate a path with no vertices.");
					return;
				}

				path.Add(path[0]);
				path.RemoveAt(0);
			}

			public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection, bool allowOnLine = true)
			{
				intersection = Vector2.zero;
				var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
				if (d == 0.0f)
				{
					return false;
				}

				var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
				var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;
				if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
				{
					return false;
				}

				if (!allowOnLine && (u == 0 || u == 1 || v == 0 || v == 1)) return false;
				intersection.x = p1.x + u * (p2.x - p1.x);
				intersection.y = p1.y + u * (p2.y - p1.y);
				return true;
			}

			public static Rect aabbFromPath(List<Vector2> path)
			{
				if (path.Count < 1) throw new ArgumentException("Path must have length 1 at least!");
				float minX, maxX, minY, maxY;
				minX = maxX = path[0].x;
				minY = maxY = path[0].y;
				foreach (var v in path)
				{
					if (v.x < minX) minX = v.x;
					if (v.x > maxX) maxX = v.x;
					if (v.y < minY) minY = v.y;
					if (v.y > maxY) maxY = v.y;
				}

				return Rect.MinMaxRect(minX, minY, maxX, maxY);
			}

			public static bool IsPointInsidePolygon(List<Vector2> polygon, Vector2 point)
			{
				if (polygon == null) throw new ArgumentNullException("Polygon cannot be null");
				if (point == null) throw new ArgumentNullException("Point cannot be null");
				var aabb = aabbFromPath(polygon);
				//Cast a ray from point to positive x
				var ray = new Segment(point, new Vector2(aabb.max.x, point.y));
				//Count intersections with path
				var intersections = 0;
				for (var i = 0; i < polygon.Count; ++i)
				{
					var p1 = polygon[i];
					var p2 = polygon[(i + 1) % polygon.Count];
					Vector2 unused;
					if (LineSegmentsIntersection(p1, p2, ray.a, ray.b, out unused))
					{
						++intersections;
					}
				}

				return intersections % 2 == 1;
			}

			public static bool IsVector3ApproximatelyEqual(Vector3 a, Vector3 b)
			{
				return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
			}

			public static bool IsPolygonClockwise(List<Vector2> polygon)
			{
				if (polygon == null) throw new ArgumentNullException("Polygon must not be null!");
				if (polygon.Count < 3) throw new ArgumentException("Polygon cannot have less than 3 vertices!");
				float sum = 0;
				for (var i = 0; i < polygon.Count; ++i)
				{
					var p1 = polygon[i];
					var p2 = polygon[(i + 1) % polygon.Count];
					sum += (p2.x - p1.x) * (p2.y + p1.y);
				}

				return sum >= 0;
			}

			public static bool IsTriangleClockwise(Triangle tri)
			{
				return IsPolygonClockwise(new List<Vector2> { tri.a, tri.b, tri.c });
			}

			public static Triangle CertifyWindingOrder(Triangle tri, bool clockwise)
			{
				if (IsTriangleClockwise(tri) != clockwise)
				{
					//invert winding order of triangle
					var cache = tri.b;
					tri.b = tri.c;
					tri.c = cache;
				}

				return tri;
			}

			public static void CertifyWindingOrder(ref List<Triangle> tris, bool clockwise)
			{
				if (tris == null) throw new ArgumentNullException("Tris is null; invalid operation.");
				for (var i = 0; i < tris.Count; ++i)
				{
					tris[i] = CertifyWindingOrder(tris[i], clockwise);
				}
			}

			public static List<Triangle> Triangulate(List<Vector2> pathSource, int subsequentCalls = 0)
			{
				if (pathSource == null) throw new ArgumentNullException("PathSource must not be null!");
				if (pathSource.Count < 3)
				{
					Debug.LogWarning("Cannot triangulate path with less than 3 vertices. Path has " + pathSource.Count + " vertices.");
					return null;
				}

				//Copy path source over to path
				var path = new List<Vector2>();
				foreach (var vert in pathSource) path.Add(vert);
				var segment      = new Segment(path[1], path[path.Count - 1]);
				var    segmentValid = true;

				//Does the center point of the segment lie outside the polygon?
				if (!IsPointInsidePolygon(path, segment.Center))
				{
					segmentValid = false;
				}

				if (segmentValid)
				{
					//Does the segment [1, L-1] cut through the shape at any point?
					var center = segment.Center;
					for (var i = 0; i < path.Count; ++i)
					{
						var p1 = path[i];
						var p2 = path[(i + 1) % path.Count];
						Vector2 unused;
						if (LineSegmentsIntersection(p1, p2, segment.a, segment.b, out unused, false))
						{
							segmentValid = false;
							break;
						}
					}
				}

				if (!segmentValid)
				{
					//Try again from the next vertex
					if (subsequentCalls > path.Count)
					{
						//Rotated fully the vertices and never found a break. Something bad happened :'(
						Debug.LogError("Cannot triangulate path. Rotated too many times subsequently with no improvements.");
						return null;
					}

					RotatePathClockwise(ref path);
					return Triangulate(path, subsequentCalls + 1);
				}

				//Ok! the segment being valid, let's extract that triangle from the path and carry on.
				var firstTri = new Triangle(path[0], path[1], path[path.Count - 1]);
				path.RemoveAt(0);
				if (path.Count < 3)
				{
					return new List<Triangle> { firstTri };
				}
				else
				{
					//still have more than 4 vertices, keep going deeper
					var result = new List<Triangle> { firstTri };
					result.AddRange(Triangulate(path));
					return result;
				}
			}

			public static void AddVertexToMesh(ref List<Vector3> vertices, ref List<int> indices, Vector3 vertex)
			{
				for (var i = 0; i < vertices.Count; ++i)
				{
					if (IsVector3ApproximatelyEqual(vertices[i], vertex))
					{
						indices.Add(i);
						return;
					}
				}

				vertices.Add(vertex);
				indices.Add(vertices.Count - 1);
			}

			public static void AddTriangleToMesh(ref List<Vector3> vertices, ref List<int> indices, Triangle tri, float z, bool clockwise)
			{
				tri = CertifyWindingOrder(tri, clockwise); //make triangle clockwise (or counterclockwise)
				//Add the three vertices, in order.
				AddVertexToMesh(ref vertices, ref indices, new Vector3(tri.a.x, tri.a.y, z));
				AddVertexToMesh(ref vertices, ref indices, new Vector3(tri.b.x, tri.b.y, z));
				AddVertexToMesh(ref vertices, ref indices, new Vector3(tri.c.x, tri.c.y, z));
			}

			public static void AddTrianglesToMesh(ref List<Vector3> vertices, ref List<int> indices, List<Triangle> tris, float z, bool clockwise)
			{
				foreach (var tri in tris) AddTriangleToMesh(ref vertices, ref indices, tri, z, clockwise);
			}
		}
	}
	
    public interface IZone<out T>
    {
        IReadOnlyCollection<T>  Content { get; }
    }

    public abstract class Zone<T> : MonoBehaviour, IZone<T>
    {
        private HashSet<T>             m_Content = new HashSet<T>();
        public  IReadOnlyCollection<T> Content => m_Content;

        public event Action<T> OnEnter;
        public event Action<T> OnLeave;

        // =======================================================================
        protected abstract T _extract(Collider other);

        private void OnTriggerEnter(Collider other)
        {
            var content = _extract(other);
            if (!content.IsNull())
                if (m_Content.Add(content))
                    OnEnter?.Invoke(content);
        }

        private void OnTriggerExit(Collider other)
        {
            var content = _extract(other);
            if (!content.IsNull())
                if (m_Content.Remove(content))
                    OnLeave?.Invoke(content);
        }

        private void OnDisable()
        {
            m_Content.Clear();
        }
    }
}