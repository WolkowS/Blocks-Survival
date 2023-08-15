using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(MatProp))]
    public class MatPropDrawer : PropertyDrawer
    {
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        { 
            var name = property.FindPropertyRelative(nameof(MatProp.Name));
            var hash = property.FindPropertyRelative(nameof(MatProp.Hash));
            
            EditorGUI.PropertyField(position, name, label);
            hash.intValue = Shader.PropertyToID(name.stringValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}