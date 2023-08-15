using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class CellPos
    {
        protected const uint k_MaskX = ((uint)0x0000ffff) << 16;
        protected const uint k_MaskY = 0x0000ffff;

        [SerializeField]
        protected uint _index;

        protected short _x;
        protected short _y;

        public short X
        {
            set
            {
                _x     =  value;
                _index &= k_MaskY;
                _index |= ((uint)value) << 16;
            }
            get => _x;
        }

        public short Y
        {
            set
            {
                _y     =  value;
                _index &= k_MaskX;
                _index |= ((uint)value) & k_MaskY;
            }
            get => _y;
        }
    }

    [Serializable]
    public class CellPos<T> : CellPos, IDisposable
    {
        public static Dictionary<uint, CellPos<T>> s_Map = new Dictionary<uint, CellPos<T>>();

        private T          _owner;
        private CellPos<T> _prev;
        private CellPos<T> _next;

        public T Owner => _owner;

        // =======================================================================
        public static IEnumerable<CellPos<T>> Scan(in Vector2Int cell) => Scan(cell.ToIndex());
        public static IEnumerable<CellPos<T>> Scan(uint index)
        {
            if (s_Map.TryGetValue(index, out var cellPos))
            {
                var result = cellPos;
                do
                {
                    yield return result;
                    result = result._next;
                } while (result != null);
            }
        }

        public static CellPos<T> Ping(in Vector2Int cell) => Ping(cell.ToIndex());
        public static CellPos<T> Ping(uint index)
        {
            if (s_Map.TryGetValue(index, out var cellPos))
                return cellPos;

            return null;
        }

        public void Move(in Vector2Int cell) => Move((short)cell.x, (short)cell.y);
        public void Move(short x, short y)
        {
            if (X == x && Y == y)
                return;

            Dispose();

            X = x;
            Y = y;

            Deploy();
        }

        public void Init(T owner, in Vector2Int pos)
        {
            _owner = owner;

            _x     = (short)pos.x;
            _y     = (short)pos.y;
            _index = pos.ToIndex();

            Deploy();
        }

        public void Deploy()
        {
            if (s_Map.TryGetValue(_index, out var cell))
            {
                // add as last child
                cell._getLast()._link(this);
                return;
            }

            // add as root
            s_Map.Add(_index, this);
        }

        public void Dispose()
        {
            var nextNull = _next == null;
            var prevNull = _prev == null;

            if (nextNull && prevNull)
            {
                s_Map.Remove(_index);
                return;
            }

            if (!nextNull)
                _next._prev = _prev;

            if (!prevNull)
                _prev._next = _next;

            _prev = null;
            _next = null;
        }

        // -----------------------------------------------------------------------
        private void _link(CellPos<T> child)
        {
            _next       = child;
            child._prev = this;
        }

        private CellPos<T> _getLast()
        {
            var result = this;

            while (result._next != null)
                result = result._next;

            return result;
        }
    }
}