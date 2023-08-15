using System.Linq;
using UnityEngine;

namespace CoreLib.Tween
{
    public abstract class OscillatorBase : MonoBehaviour, IRefGet<float>, IRefGet<Vector2>, IRefGet<Vector3>
    {
        public virtual float   Value  => 0f;
        public virtual Vector2 Value2 => new Vector2(Value, Value);
        public virtual Vector3 Value3 => new Vector3(Value, Value, Value);
        
        Vector2 IRefGet<Vector2>.Value => Value2;
        Vector3 IRefGet<Vector3>.Value => Value3;
        
        // =======================================================================
        internal virtual void OnValidate()
        {
            var tween = GetComponents<Tween>().FirstOrDefault(n => n.m_Input == null);
            if (tween != null)
            {
                tween.m_Input = this;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(tween);
#endif
            }
        }

    }
}