using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(StateMachine<>), true)]
    [CustomPropertyDrawer(typeof(TinyFSM<>), true)]
    public class StateMachineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stateMachine = property.GetSerializedValue<object>();
            var stateLable = stateMachine
                             .GetType()
                             .GetMethod("CurrentLabel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             ?.Invoke(stateMachine, Array.Empty<object>())
                             ?.ToString();
         
            EditorGUI.PropertyField(position, property, label, true);

            if (stateLable != null)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                GUI.enabled = false;
                var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight).Field();
                GUI.Button(rect, new GUIContent(stateLable));
                GUI.enabled = true;
                EditorGUI.indentLevel = indent;
            }
        }
	
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}