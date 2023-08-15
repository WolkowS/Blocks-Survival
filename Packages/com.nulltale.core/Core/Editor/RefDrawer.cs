using NaughtyAttributes;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(Ref<>), true)]
    [CustomPropertyDrawer(typeof(Signal<>), true)]
    public class RefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var refValue = property.FindPropertyRelative("m_Value");
            EditorGUI.PropertyField(position, refValue, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var refValue = property.FindPropertyRelative("m_Value");
            return EditorGUI.GetPropertyHeight(refValue);
        }
    }
}