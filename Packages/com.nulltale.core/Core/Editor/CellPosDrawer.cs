using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(CellPos), true)]
    public class CellPosDrawer : PropertyDrawer
    {
        // private static readonly Vector2Int k_Offset = new Vector2Int((int)(ushort.MaxValue >> 1), (int)(ushort.MaxValue >> 1));
        private static readonly Vector2Int k_Offset = Vector2Int.zero;
        
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index    = property.FindPropertyRelative("_index");
            var indexVal = ((uint)index.longValue).ToVector2Int() - k_Offset;
            
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.Vector2IntField(position, label, indexVal) + k_Offset;
            if (EditorGUI.EndChangeCheck())
            {
                var cp = property.GetSerializedValue<CellPos>();
                cp.X = (short)result.x;
                cp.Y = (short)result.y;
                property.serializedObject.ApplyModifiedProperties();
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}