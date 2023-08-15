using System.Collections.Generic;
using System.Linq;
using AStar;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace CoreLib
{
    public class SplineAgent : MonoBehaviour
    {
        public  SplineContainer _path;
        private Spline          _spline;
        private SplinePath      _pathCache;
        
        public Transform _goal;
        
        public float _splinePos;
        public float _speed;
        
        private List<Command> _commands = new List<Command>();

        public UnityEvent _onReached;

        // =======================================================================
        public class SplineExplorer : IExplorer<SplineNode>
        {
            public static SplineExplorer Instance = new SplineExplorer();
            
            // =======================================================================
            public IEnumerable<SplineNode> GetNeighbours(SplineNode node)
            {
                return node.Neighbours();
            }

            public float GetPathCost(SplineNode start, SplineNode goal)
            {
                if (start._next == goal)
                    return start._lenght;
                
                return 0f;
            }

            public float GetShortestPath(SplineNode start, SplineNode goal)
            {
                return Vector3.Distance(start._pos, goal._pos);
            }

            public bool Reachable(SplineNode start, SplineNode goal)
            {
                return true;
            }

            public bool Passable(SplineNode node)
            {
                return true;
            }
        }

        public class SplineNode
        {
            public float            _lenght;
            public float            _time;
            public Vector3          _pos;
            public SplineKnotIndex  _index;
            public Spline           _spline;
            public SplineNode       _next;
            public SplineNode       _prev;
            public List<SplineNode> _links = new List<SplineNode>();

            // =======================================================================
            public IEnumerable<SplineNode> Neighbours()
            {
                if (_next != null)
                    yield return _next;
                
                if (_prev != null)
                    yield return _prev;

                foreach (var sn in _links)
                    yield return sn;
            }
        }
        
        public class SplineGraph
        {
            public List<SplineNode> _nodes = new List<SplineNode>();
            
            // =======================================================================
            public void Init(SplineContainer sc)
            {
                _nodes.Clear();

                // allocate
                for (var i = 0; i < sc.Splines.Count; i++)
                {
                    var spline = sc.Splines[i];
                    var lenght   = 0f;
                    for (var n = 0; n < spline.Count; n++)
                    {
                        var knot = spline[n];
                        var node = new SplineNode()
                        {
                            _lenght = spline.GetCurveLength(n),
                            _time   = lenght / spline.GetLength(),
                            _pos    = knot.Position,
                            _index  = new SplineKnotIndex(i, n),
                            _spline = spline,
                        };
                        lenght += node._lenght;
                        _nodes.Add(node);
                    }
                }

                // link
                var index = 0;
                foreach (var node in _nodes)
                {
                    if (node._index.Knot != node._spline.Count - 1)
                        node._next = _nodes[index + 1];
                    
                    if (node._index.Knot - 1 >= 0)
                        node._prev = _nodes[index - 1];
                    
                    foreach (var splineKnotIndex in sc.KnotLinkCollection.GetKnotLinks(node._index))
                    {
                        var nodeLink = _nodes.FirstOrDefault(n => n._index == splineKnotIndex);
                        if (nodeLink != null)
                            node._links.Add(nodeLink);
                    }
                    index ++;
                }
            }
            
            public SplineNode Get(int spline, int knot)
            {
                return _nodes.FirstOrDefault(n => n._index.Spline == spline && n._index.Knot == knot);
            }
            
            public SplineNode Get(SplineKnotIndex index)
            {
                return _nodes.FirstOrDefault(n => n._index.Spline == index.Spline && n._index.Knot == index.Knot);
            }
        }

        public abstract class Command
        {
            public abstract bool IsMet();
            public abstract void Apply(SplineAgent agent);
        }
        
        private class SnapCommand : Command
        {
            public float _time;
            
            // =======================================================================
            public override bool IsMet() => true;

            public override void Apply(SplineAgent agent)
            {
                agent.Snap();
            }
        }
        
        private class LandInCommand : Command
        {
            public int _spline;
            public int _knot;
            
            // =======================================================================
            public override bool IsMet() => true;

            public override void Apply(SplineAgent agent)
            {
                agent.SetSpline(_spline);
                agent.SetKnot(_knot);
            }
        }

        private class MoveCommand : Command
        {
            private float _current;
            public  float _dest;

            // =======================================================================
            public override bool IsMet() => (_dest - _current).Abs() < 0.01f;

            public override void Apply(SplineAgent agent)
            {
                agent._move(_dest);
                _current = agent._splinePos;
            }
        }

        // =======================================================================
        [Button]
        public void Move()
        {
            SetDestination(_goal.position);
        }
        
        public void SetDestination(Vector3 target)
        {
            var sg = new SplineGraph();
            sg.Init(_path);
            
            var pos  = sg.Get(_path.IndexOf(_spline), _spline.GetClosestKnot(_splinePos));
            var dest = sg.Get(_path.GetClosestKnot(target));
            
            var path = Pathfinder.FindPath(SplineExplorer.Instance, pos, dest).Execute();
            
            _commands.Clear();
            var pathNodes = path.Path.ToArray();
            
            for (var n = 0; n < pathNodes.Length - 1; n++)
            {
                var node = pathNodes[n];
                var next = pathNodes[n + 1];
                if (node._spline != next._spline)
                {
                    _commands.Add(new MoveCommand() { _dest = node._time });
                    _commands.Add(new LandInCommand() { _spline = next._index.Spline, _knot = next._index.Knot });
                }
            }
            
            _path.GetClosestPos(_goal.transform.position, out _, out var destTime);
            _commands.Add(new MoveCommand() { _dest = destTime });
        }
        
        public void SetSpline(int spline)
        {
            _spline = _path[spline];
            _pathCache = _spline.GetSplinePath();
        }
        
        public void SetKnot(int knotIndex)
        {
            transform.position = _spline[knotIndex].Position;
            _splinePos = _spline.GetKnotTime(knotIndex);
        }
        
        [Button]
        public void Snap()
        {
            transform.position = _path.GetClosestPos(transform.position, out var spline, out _splinePos);
            SetSpline(_path.IndexOf(spline));
        }

        private void Update()
        {
            if (_commands.IsEmpty())
                return;

            var cmd = _commands.First();
            cmd.Apply(this);
            
            if (cmd.IsMet())
            {
                _commands.Remove(cmd);
                if (_commands.IsEmpty())
                    _onReached.Invoke();
            }
        }

        // =======================================================================
        private void _move(float dest)
        {
            _splinePos         = _splinePos.MoveTowards(dest, (_speed * Time.deltaTime) / _pathCache.GetLength());
            transform.position = _pathCache.EvaluatePosition(_splinePos);
        }
    }
}