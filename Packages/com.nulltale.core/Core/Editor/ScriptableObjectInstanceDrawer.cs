using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.ExposedValues
{
    [CustomPropertyDrawer(typeof(SOInstance))]
    public class ScriptableObjectInstanceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position.WithHeight(EditorGUIUtility.singleLineHeight), property, label);
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(5), property.isExpanded, GUIContent.none, toggleOnLabelClick: true);
                property.DrawObjectReference(position);
            }

            // delete
            if (property.objectReferenceValue != null && _rightClick())
            {
                var context = new GenericMenu();
                var options = new List<(string, Action)>();

                options.Add(new ("Destroy", () =>
                {
                    if (property.objectReferenceValue == null)
                        return;

                    Object.DestroyImmediate(property.objectReferenceValue);
                    property.objectReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }));

                foreach (var option in options)
                    context.AddItem(new GUIContent(option.Item1), false, option.Item2.Invoke);

                // show menu
                if (options.IsEmpty() == false)
                    context.ShowAsContext();
            }

            // create new global value
            if (property.objectReferenceValue == null && _rightClick())
            {
                var context = new GenericMenu();
                var options = new List<(string, Action)>();

                var fieldType = property.GetFieldType();
                var types = TypeCache.GetTypesDerivedFrom(fieldType).Prepend(fieldType).Where(n =>
                {
                    if (n.IsAbstract)
                        return false;
                    
                    if (n.IsGenericTypeDefinition)
                        return false;

                    return true;
                }).ToArray();

                foreach (var type in types)
                {
                    options.Add(new (type.Name, () =>
                    {
                        property.objectReferenceValue = ScriptableObject.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    }));
                }

                foreach (var option in options)
                    context.AddItem(new GUIContent(option.Item1), false, option.Item2.Invoke);

                // show menu
                if (options.IsEmpty() == false)
                    context.ShowAsContext();
            }

            // ===================================
            bool _rightClick()
            {
                return Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 1;
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return property.GetObjectReferenceHeight();
        }
    }
}