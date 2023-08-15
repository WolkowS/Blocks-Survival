using UnityEditor;
using UnityEngine;

namespace CoreLib.Midi
{
    [CustomPropertyDrawer(typeof(MidiOutput.Key))]
    public class MidiKeyDrawer : PropertyDrawer
    {
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var key = property.FindPropertyRelative(nameof(MidiOutput.Key._value));
            EditorGUI.PropertyField(position, key, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}