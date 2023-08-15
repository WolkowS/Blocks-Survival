using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(Switch), true)]
    public class SwitchDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lockValue = property.FindPropertyRelative("m_Value");
            EditorGUI.BeginChangeCheck();
            var toggle = EditorGUI.Toggle(position, label, lockValue.intValue > 0);
            if (EditorGUI.EndChangeCheck())
                lockValue.intValue = toggle ? 1 : 0;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var refValue = property.FindPropertyRelative("m_Value");
            return EditorGUI.GetPropertyHeight(refValue);
        }
    }
}