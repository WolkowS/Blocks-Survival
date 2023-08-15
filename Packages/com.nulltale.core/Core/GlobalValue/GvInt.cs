using System;
using System.Globalization;
using UnityEngine;

namespace CoreLib.Values
{
    public class GvInt : GlobalValuePlayable<int>
    {
        public override string ToString()
        {
            return Value.ToString();
        }
        
        internal override string Serialize()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        internal override void Deserialize(string data)
        {
            Value = int.Parse(data);
        }
        
        public void Decrement()
        {
            Value --;
        }
        
        public void Increment()
        {
            Value ++;
        }
        
        public void Add(MonoBehaviour iRef)
        {
            Value += ((IRefGet<int>)iRef).Value;
        }
        
        public void Sub(MonoBehaviour iRef)
        {
            Value -= ((IRefGet<int>)iRef).Value;
        }
        
        public void Add(int val)
        {
            Value += val;
        }
        
        public void Set(int val)
        {
            Value = val;
        }
        
        public void Set(GvInt val)
        {
            Value = val.Value;
        }
        
        public void SetRound(float val)
        {
            Value = val.RoundToInt();
        }

        private SortedCollection<PlayableValueHandle<int>> m_ApplySequence = new SortedCollection<PlayableValueHandle<int>>(PriorityRelationalComparer.Instance);

        // =======================================================================
        public class Handle : PlayableValueHandle<int>
        {
            public override void Apply(ref int value)
            {
                switch (m_Mode)
                {
                    case SetMode.Floor:
                    {
                        var result = _getOverride();
                        if (value > result)
                            value = result;
                    } break;
                    case SetMode.Ceil:
                    {
                        var result = _getOverride();
                        if (value < result)
                            value = result;
                    } break;
                    case SetMode.Override:
                    {
                        var result = _getOverride();
                        value = result;
                    } break;
                    case SetMode.Add:
                    {
                        value += (m_Value * m_Weight).RoundToInt();
                    } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // -----------------------------------------------------------------------
                int _getOverride()
                {
                    return Mathf.Lerp(m_Blend.Value, m_Value, m_Weight).RoundToInt();
                }
            }

            public Handle(GlobalValuePlayable<int> owner, SortedCollection<PlayableValueHandle<int>> applySequence) : base(owner, applySequence) { }
        }

        // =======================================================================
        internal override IPlayableValueHandle OpenHandle()
        {
            return new Handle(this, m_ApplySequence);
        }

        internal override void CloseHandle(IPlayableValueHandle handle)
        {
            m_ApplySequence.Remove((PlayableValueHandle<int>)handle);
        }

        internal override int GetPlayableValue()
        {
            var value = CleanValue;
            foreach (var handle in m_ApplySequence)
                handle.Apply(ref value);

            return value;
        }
    }
}