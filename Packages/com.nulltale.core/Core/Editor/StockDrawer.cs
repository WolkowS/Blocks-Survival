using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(StockInt), true)]
    public class StockDrawer : PropertyDrawer
    {
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var val = property.FindPropertyRelative("_stock");
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.Vector2IntField(position, label, val.vector2IntValue);
            if (EditorGUI.EndChangeCheck())
            {
                result.x            = result.x.Clamp(0, result.y);
                val.vector2IntValue = result;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}