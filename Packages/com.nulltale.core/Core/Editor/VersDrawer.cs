using System;
using CoreLib.Values;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(Vers<>))]
    public class VersDrawer : PropertyDrawer
    {
        private const float k_ToggleWidth = 22;

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent icon; 
            var mode = property.FindPropertyRelative("m_Mode");
            position.width -= k_ToggleWidth;
            switch ((Vers<int>.Mode)mode.intValue)
            {
                case Vers<int>.Mode.Override:
                {
                    icon = new GUIContent("C");
                    var globalValueOverride = property.FindPropertyRelative("m_Override");
                    EditorGUI.PropertyField(position, globalValueOverride, label, true);
                } break;
                case Vers<int>.Mode.Global:
                {
                    icon = new GUIContent("G");
                    var globalValue = property.FindPropertyRelative("m_GlobalValue");
                    EditorGUI.PropertyField(position, globalValue, label, true);
                } break;
                case Vers<int>.Mode.Ref:
                {
                    icon = new GUIContent("R");
                    var refValue = property.FindPropertyRelative("m_Ref");
                    EditorGUI.PropertyField(position, refValue, label, true);
                } break;
                case Vers<int>.Mode.Link:
                {
                    icon = new GUIContent("L");
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
                if (mode.intValue > 3)
                    mode.intValue = 0;
                if (mode.intValue < 0)
                    mode.intValue = 3;
            }

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (Vers<int>.Mode)property.FindPropertyRelative("m_Mode").intValue switch
            {
                Vers<int>.Mode.Override => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Override")),
                Vers<int>.Mode.Global   => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_GlobalValue")),
                Vers<int>.Mode.Ref      => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Ref")),
                Vers<int>.Mode.Link     => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Resolver")),
                _                       => throw new ArgumentOutOfRangeException()
            };
        }
    }
}