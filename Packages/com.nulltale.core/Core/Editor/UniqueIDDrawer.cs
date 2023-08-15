using System;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(UniqueId), true)]
    public class UniqueIDDrawer : PropertyDrawer 
    {
        private static string k_GenerateGuidEvent = "GenerateGuid";

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var uid = property.FindPropertyRelative("m_GUID");
		
		    // assign new guid
            if (string.IsNullOrEmpty(uid.stringValue) ||     // reset if null
                ((Event.current.type == EventType.MouseDown) // or double click)
                && (Event.current.button == 0)
                && (Event.current.clickCount == 2))
                && new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight).Contains(Event.current.mousePosition))
            {
                uid.stringValue = Guid.NewGuid().ToString();
                uid.serializedObject.ApplyModifiedProperties();
            }

            // repaint
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.LabelField(position, new GUIContent("ID"), new GUIContent(uid.stringValue, "Double click to generate new GUID"));
            }

            if (Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == k_GenerateGuidEvent)
                {
                    uid.stringValue = Guid.NewGuid().ToString();
                    uid.serializedObject.ApplyModifiedProperties();
                }
            }

            if (Event.current.type == EventType.ContextClick && position.Contains(Event.current.mousePosition))
            {
                var context = new GenericMenu();
		 
                context.AddItem(new GUIContent ("Generate"), false, () => 
                {
                    uid.stringValue = Guid.NewGuid().ToString();
                    uid.serializedObject.ApplyModifiedProperties();
                });
                context.AddItem(new GUIContent ("Copy"), false, () => EditorGUIUtility.systemCopyBuffer = uid.stringValue);
                context.AddItem(new GUIContent ("Paste"), false, () =>
                {
                    uid.stringValue = EditorGUIUtility.systemCopyBuffer;
                    uid.serializedObject.ApplyModifiedProperties();
                });
		 
                context.ShowAsContext();
            }
		
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}