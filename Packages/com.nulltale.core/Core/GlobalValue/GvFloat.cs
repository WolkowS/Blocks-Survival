using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.Values
{
    public class GvFloat : GlobalValuePlayable<float>
    {
        private SortedCollection<PlayableValueHandle<float>> m_ApplySequence = new SortedCollection<PlayableValueHandle<float>>(PriorityRelationalComparer.Instance);

        // =======================================================================
        public class Handle : PlayableValueHandle<float>
        {
            public override void Apply(ref float value)
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
                        value += m_Value * m_Weight;
                    } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // -----------------------------------------------------------------------
                float _getOverride()
                {
                    return Mathf.Lerp(m_Blend.Value, m_Value, m_Weight);
                }
            }

            public Handle(GlobalValuePlayable<float> owner, SortedCollection<PlayableValueHandle<float>> applySequence) : base(owner, applySequence) { }
        }

        // =======================================================================
        internal override IPlayableValueHandle OpenHandle()
        {
            return new Handle(this, m_ApplySequence);
        }

        internal override void CloseHandle(IPlayableValueHandle handle)
        {
            m_ApplySequence.Remove((PlayableValueHandle<float>)handle);
        }

        internal override float GetPlayableValue()
        {
            var value = CleanValue;
            foreach (var handle in m_ApplySequence)
                handle.Apply(ref value);

            return value;
        }

        internal override string Serialize()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        internal override void Deserialize(string data)
        {
            Value = float.Parse(data, CultureInfo.InvariantCulture.NumberFormat);
        }

        public void Add(float val)
        {
            Value += val;
        }
        
        public void Add(MonoBehaviour iRef)
        {
            Value += ((IRefGet<float>)iRef).Value;
        }
        
        public void Add(Object iRef)
        {
            Value += ((IRefGet<float>)iRef).Value;
        }
        
        public void Sub(float val)
        {
            Value -= val;
        }
        
        public void Sub(MonoBehaviour iRef)
        {
            Value -= ((IRefGet<float>)iRef).Value;
        }
        
        public override string ToString()
        {
            return Value.ToString("F", Core.k_NumberFormat);
        }
    }
}