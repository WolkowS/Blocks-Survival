using System;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(EnumAttribute))]
    public class EnumPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.Generic
                ? EditorGUIUtility.singleLineHeight
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            Draw(rect, property, label, ((EnumAttribute)attribute));
        }

        public static void Draw(Rect rect, SerializedProperty property, GUIContent label, EnumAttribute attr)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = (int)Convert.ChangeType(EditorGUI.EnumPopup(rect, label, (Enum)Enum.ToObject(attr._type, property.intValue)), attr._type);
            }
            else
            if (property.propertyType == SerializedPropertyType.Generic)
            {
                // Draw Ref
                var refValue = property.FindPropertyRelative("m_Value");
                refValue.intValue = (int)Convert.ChangeType(EditorGUI.EnumPopup(rect, label, (Enum)Enum.ToObject(attr._type, refValue.intValue)), attr._type);
            }
            else
            {
                var message = attr.GetType().Name + " can be used only on int";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }
    }
}