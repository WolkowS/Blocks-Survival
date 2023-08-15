using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Type.Get
            var typeNameProperty = property.FindPropertyRelative("m_TypeName");
		
            // select type btn
            GUI.Label(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), property.displayName);

            if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height), 
                getDisplayTypeName(typeNameProperty.stringValue)))
            {
                var typeList = new List<Type>();
                
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    filterTypes(assembly, typeList);
                typeList.Sort((x, y) => string.CompareOrdinal(x.FullName, y.FullName));

                ObjectPickerWindow.Show(picked =>
                {
                    typeNameProperty.stringValue = (picked as Type).AssemblyQualifiedName;
                    property.serializedObject.ApplyModifiedProperties();
                }, null, typeList, 0, n => new GUIContent(n.FullName), "Select a type");
            }

            // -----------------------------------------------------------------------
            string getDisplayTypeName(string assemblyQualifiedName)
            {
                if (string.IsNullOrEmpty(assemblyQualifiedName))
                    return "";

                return assemblyQualifiedName.Remove(assemblyQualifiedName.IndexOf(','));
            }
            
            void filterTypes(Assembly assembly, List<Type> output) 
            {
                var filters = fieldInfo
                    .GetCustomAttributes(typeof(TypeReferenceFilterAttribute), false)
                    .Cast<TypeReferenceFilterAttribute>()
                    .ToList();

                foreach (var type in assembly.GetTypes()) 
                {
                    if (type.IsVisible == false)
                        continue;

                    /*if (type.IsSerializable == false)
					continue;*/

                    if (filterCheck(type) == false)
                        continue;

                    output.Add(type);
                }

                bool filterCheck(Type type)
                {
                    foreach (var filter in filters)
                        if (filter.Verify(type) == false)
                            return false;

                    return true;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}