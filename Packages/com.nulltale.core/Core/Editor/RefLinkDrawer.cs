using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(RefLink<>), true)]
    [CustomPropertyDrawer(typeof(RefLinkId<>), true)]
    public class RefLinkDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = property.FindPropertyRelative("_obj");
            var path = property.FindPropertyRelative("_path");
            var id = property.FindPropertyRelative("_id");
            EditorGUI.LabelField(position.GuiField(0), label);
            EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 0), obj, GUIContent.none);
            
            if (id != null)
            {
                property.isExpanded = EditorGUI.Foldout(position.GuiField(0), property.isExpanded, GUIContent.none, false);
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(position.GuiField(1), id);
                    EditorGUI.indentLevel--;
                }
            }
            
            if (obj.objectReferenceValue is GameObject go)
            {
                obj.objectReferenceValue = go.GetComponents<Component>().FirstOrDefault(_hasRefField);
                if (obj.objectReferenceValue == null)
                    obj.objectReferenceValue = go.GetComponent<RefBus>();
            }
                
            if (obj.objectReferenceValue == null)
            {
                //path.stringValue = EditorGUI.TextField(position.GuiField(0).Field().WidthSegment(2, 1, EditorGUIUtility.standardVerticalSpacing), GUIContent.none, path.stringValue);
                EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 1, EditorGUIUtility.standardVerticalSpacing), path, GUIContent.none);
                return;
            }
            
            var objVal = obj.objectReferenceValue;
            while (objVal is RefBus rb)
                objVal = rb.Link;
            
            var isSelfLink = objVal == property.serializedObject.targetObject;
            
            var refType = property.GetSerializedValue<object>().GetType().GetGenericTypeArgument(typeof(RefLink<>));
            var options = _getOptions().ToArray();
            
            var index = Array.IndexOf(options, path.stringValue);
            if (index < 0 && _isRef(objVal) && path.stringValue == string.Empty)
                index = Array.IndexOf(options, "Self");
                        
            if (index < 0)
            {
                index = 0;
                options = options.Prepend(path.stringValue).ToArray();
                GUI.color = Color.red;
            }
            
            if (property.hasMultipleDifferentValues == false)
            {
                path.stringValue = options[EditorGUI.Popup(position.GuiField(0).Field().WidthSegment(2, 1, EditorGUIUtility.standardVerticalSpacing), index, options.Select(ObjectNames.NicifyVariableName).ToArray())];
                if (path.stringValue == "Self")
                    path.stringValue = string.Empty;
            }
            else
            {
                EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 1, EditorGUIUtility.standardVerticalSpacing), path, GUIContent.none);
            }
            
            
            GUI.color = Color.white;

            // -----------------------------------------------------------------------
            IEnumerable<string> _getOptions()
            {
                
                if (_isRef(objVal))
                    yield return ("Self");

                foreach (var field in objVal.GetType()
                                     .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                     .Where(n =>
                                     {
                                         if (n.FieldType.Implements<IRef>() == false)
                                             return false;

                                         if (isSelfLink)
                                         {
                                             if (property.propertyPath == n.Name)
                                                 return false;
                                         }
                   
                                         return n.FieldType.HasGetGenericTypeArgument(typeof(IRefGet<>), refType);
                                     })
                                     .Select(n => n.Name))
                {
                    yield return field;
                }
            }
        }
        
        private bool _isRef(object obj)
        {
            return obj is IRef;
        }
        
        private bool _hasRefField(object obj)
        {
            return obj.GetType()
                      .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                      .Any(n => n.FieldType.Implements<IRef>());
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasId = property.FindPropertyRelative("_id");
            var lines = 1;
            if (hasId != null && property.isExpanded)
                lines ++;
            
            return lines * EditorGUIUtility.singleLineHeight;
        }
    }
}