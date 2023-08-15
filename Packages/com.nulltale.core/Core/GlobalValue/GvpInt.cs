using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Scripting;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Values
{
    public class GvpInt : GlobalValueProgress<int>
    {
        public enum Mode
        {
            Custom,
            Curve,
            Inc,
            //Csv
        }
        
        public Mode             _mode;
        [ShowIf(nameof(_mode), Mode.Curve)]
        public int              _count;
        [ShowIf(nameof(_mode), Mode.Curve)]
        public AnimationCurve01 _lerp;
        [ShowIf(nameof(_mode), Mode.Curve)]
        public Vector2Int       _range;
        [ShowIf(nameof(_mode), Mode.Inc)]
        public List<int>        _inc;

        [Button]
        public void Bake()
        {
            switch (_mode)
            {
                case Mode.Custom:
                    break;
                case Mode.Curve:
                {
                    _level.Clear();
                    var range = (float)(_range.y - _range.x);
                    
                    for (var n = 0; n < _count; n++)
                        _level.Add(_range.x + (range * _lerp.Evaluate(n / (float)(_count - 1))).RoundToInt());
                } break;
                case Mode.Inc:
                {
                    _level.Clear();
                    var initial = 0;
                    for (var n = 0; n < _inc.Count; n++)
                    {
                        initial += _inc[n];
                        _level.Add(initial);
                    }
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void Assign(IntContainer cont)
        {
            cont.Value = Level.Value;
        }
        
        public void Assign(GvInt cont)
        {
            cont.Value = Level.Value;
        }
        
        public override void Init()
        {
#if UNITY_EDITOR
            Bake();
#endif
            base.Init();
        }
    }
}