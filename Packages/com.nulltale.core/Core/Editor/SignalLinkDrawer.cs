using System;
using CoreLib.Values;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(SignalLink<>))]
    public class SignalLinkDrawer : PropertyDrawer
    {
        private const float k_ToggleWidth = 22;

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent icon; 
            var        mode = property.FindPropertyRelative("m_Mode");
            position.width -= k_ToggleWidth;
            switch ((SignalLink<int>.Mode)mode.intValue)
            {
                case SignalLink<int>.Mode.Ref:
                {
                    icon = new GUIContent("L");
                    var refValue = property.FindPropertyRelative("m_Ref");
                    EditorGUI.PropertyField(position, refValue, label, true);
                } break;
                case SignalLink<int>.Mode.Resolver:
                {
                    icon = new GUIContent("R");
                    var resValue = property.FindPropertyRelative("m_Resolver");
                    EditorGUI.PropertyField(position, resValue, label, true);
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var togglePos = new Rect(position.x + position.width + EditorGUIUtility.standardVerticalSpacing * 2, position.y, k_ToggleWidth - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(togglePos, icon))
            {
                mode.intValue += Event.current.button == MouseButton.Left ? 1 : -1;
                if (mode.intValue > 1)
                    mode.intValue = 0;
                if (mode.intValue < 0)
                    mode.intValue = 1;
            }

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (SignalLink<int>.Mode)property.FindPropertyRelative("m_Mode").intValue switch
            {
                SignalLink<int>.Mode.Ref      => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Ref")),
                SignalLink<int>.Mode.Resolver => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Resolver")),
                _                       => throw new ArgumentOutOfRangeException()
            };
        }
    }
}