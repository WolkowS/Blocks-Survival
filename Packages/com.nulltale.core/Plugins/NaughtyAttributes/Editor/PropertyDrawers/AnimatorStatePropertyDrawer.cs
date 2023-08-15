using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    public class AnimatorStatePropertyDrawer : PropertyDrawerBase
    {
        private const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        private const string InvalidTypeWarningMessage               = "{0} must be an int or a string";

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            AnimatorStateAttribute animatorStateAttribute  = PropertyUtility.GetAttribute<AnimatorStateAttribute>(property);
            bool                   validAnimatorController = AnimatorParamPropertyDrawer.GetAnimatorController(property, animatorStateAttribute.AnimatorName) != null;
            bool                   validPropertyType       = property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String;

            return (validAnimatorController && validPropertyType)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            AnimatorStateAttribute animatorStateAttribute = PropertyUtility.GetAttribute<AnimatorStateAttribute>(property);

            AnimatorController animatorController = AnimatorParamPropertyDrawer.GetAnimatorController(property, animatorStateAttribute.AnimatorName);
            if (animatorController == null)
            {
                DrawDefaultPropertyAndHelpBox(rect, property, InvalidAnimatorControllerWarningMessage, MessageType.Warning);
                return;
            }

            var animatorParameters = animatorController
                                     .layers
                                     .SelectMany(n => n.stateMachine.states)
                                     .Select(n => n.state)
                                     .ToList();

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorParameters);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorParameters);
                    break;
                default:
                    DrawDefaultPropertyAndHelpBox(rect, property, string.Format(InvalidTypeWarningMessage, property.name), MessageType.Warning);
                    break;
            }

            EditorGUI.EndProperty();
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> states)
        {
            int paramNameHash = property.intValue;
            int index         = 0;

            for (int i = 0; i < states.Count; i++)
            {
                if (paramNameHash == states[i].nameHash)
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(states);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            int newValue = newIndex == 0 ? 0 : states[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, List<AnimatorState> states)
        {
            string paramName = property.stringValue;
            int    index     = 0;

            for (int i = 0; i < states.Count; i++)
            {
                if (paramName.Equals(states[i].name, System.StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            string[] displayOptions = GetDisplayOptions(states);

            int    newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue = newIndex == 0 ? null : states[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<AnimatorState> states)
        {
            string[] displayOptions = new string[states.Count + 1];
            displayOptions[0] = "(None)";

            for (int i = 0; i < states.Count; i++)
            {
                displayOptions[i + 1] = states[i].name;
            }

            return displayOptions;
        }
    }
}