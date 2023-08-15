using CoreLib.Values;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(GvRef<>), true)]
    public class GlobalValueRefDrawer : PropertyDrawer
    {
        private const float k_ToggleWidth = 18;
        
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var use  = property.FindPropertyRelative(nameof(GvRef<int>._useOverride));
            var over = property.FindPropertyRelative(nameof(GvRef<int>._override));
            var glob = property.FindPropertyRelative(nameof(GvRef<int>._global));

            position.width -= k_ToggleWidth;
            
            if (use.boolValue)
                EditorGUI.PropertyField(position, over, label, true);
            else
                EditorGUI.PropertyField(position, glob, label, true);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var togglePos = new Rect(position.x + position.width + EditorGUIUtility.standardVerticalSpacing, position.y, k_ToggleWidth, EditorGUIUtility.singleLineHeight);
            use.boolValue = EditorGUI.Toggle(togglePos, GUIContent.none, use.boolValue);

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var use  = property.FindPropertyRelative(nameof(GvRef<int>._useOverride));
            var over = property.FindPropertyRelative(nameof(GvRef<int>._override));
            var glob = property.FindPropertyRelative(nameof(GvRef<int>._global));
            
            if (use.boolValue)
                return EditorGUI.GetPropertyHeight(over);
            else
                return glob.GetObjectReferenceHeight();
            
        }
    }
}