using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(RangeVec2Attribute), true)]
    public class RangeVec2Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var atr = (RangeVec2Attribute)attribute;
            
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.Vector2Field(position, label, property.vector2Value);
            if (EditorGUI.EndChangeCheck())
            {
                value.x = Mathf.Clamp(value.x, atr._xMin, atr._xMax);
                value.y = Mathf.Clamp(value.y, atr._yMin, atr._yMax);
                
                if (atr._lineal)
                {
                    if (value.x > value.y && value.x > property.vector2Value.x)
                        value.y = value.x;

                    if (value.y < value.x && value.y < property.vector2Value.y)
                        value.x = value.y;
                }
                
                property.vector2Value = value;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}