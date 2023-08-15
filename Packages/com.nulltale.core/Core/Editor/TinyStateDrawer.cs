using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(TinyState))]
    public class TinyStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop     = property.FindPropertyRelative("m_LockValue");
            var isActive = prop.intValue > 0;

            EditorGUI.BeginChangeCheck();
            var state = EditorGUI.Toggle(position, property.displayName, isActive);
            if (EditorGUI.EndChangeCheck())
            {
                if (isActive != state)
                {
                    prop.intValue = state ? 1 : 0;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}