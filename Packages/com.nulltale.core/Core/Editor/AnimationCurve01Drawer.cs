using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(AnimationCurve01), true)]
    public class AnimationCurve01Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.CurveField(position, property.FindPropertyRelative(nameof(AnimationCurve01._curve)), Color.green, new Rect(0, 0, 1, 1), label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}