using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(SoNested), true)]
    public class SoNestedDrawer : PropertyDrawer
    {
        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var e = Event.current;
            if (position.Contains(e.mousePosition) &&  e.type == EventType.ContextClick)
            {
                var fieldType = property.GetFieldType();
                var types = new List<Type>();

                types.Add(fieldType);
                types.AddRange(TypeCache.GetTypesDerivedFrom(fieldType));
                types.RemoveAll(n => n.IsAbstract || n.IsGenericTypeDefinition);

                var subAssetLink = _getAssets(types.ToArray()).Contains(property.objectReferenceValue);

                var menu = new GenericMenu();
                if (subAssetLink)
                {
                    menu.AddItem(new GUIContent($"Rename"), false, () =>
                    {
                        RenameWindow.Show(property.objectReferenceValue.name, s =>
                        {
                            if (property.objectReferenceValue.name == s)
                                return;

                            property.objectReferenceValue.name = s;
                            AssetDatabase.SaveAssets();
                        }, GUIUtility.GUIToScreenPoint(e.mousePosition));
                    });
                    menu.AddSeparator(string.Empty);
                }

                foreach (var type in types)
                    menu.AddItem(new GUIContent($"Add {type.Name}"), false, _createElementOfType, type);

                if (subAssetLink)
                {
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent($"Remove"), false, () => _removeElement(property.objectReferenceValue));
                }

                menu.ShowAsContext();

                e.Use();
            }
            
            EditorGUI.PropertyField(position.GuiField(0), property, true);
            property.isExpanded = EditorGUI.Foldout(position.GuiField(0), property.isExpanded, GUIContent.none);
            property.DrawObjectReference(position);

            // ===================================
            void _createElementOfType(object type)
            {
                RenameWindow.Show($"{property.serializedObject.targetObject.name}.{property.displayName}", s =>
                {
                    // create
                    var element = ScriptableObject.CreateInstance((Type)type);
                    element.name = s;
                    AssetDatabase.AddObjectToAsset(element, property.serializedObject.targetObject);
                    AssetDatabase.SaveAssets();

                    property.objectReferenceValue = element;
                    property.serializedObject.ApplyModifiedProperties();
                }, GUIUtility.GUIToScreenPoint(e.mousePosition));
                
            }

            void _removeElement(object element)
            {
                if (property.objectReferenceValue == (Object)element)
                {
                    property.objectReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }

                AssetDatabase.RemoveObjectFromAsset((Object)element);
                AssetDatabase.SaveAssets();
            }
            
            IEnumerable<ScriptableObject> _getAssets(Type[] ofType)
            {
                
                return property.serializedObject.GetNestedAssets()
                                    .OfType<ScriptableObject>()
                                    .Where(n => ofType.Contains(n.GetType()));
            }
        }              
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetObjectReferenceHeight();
        }
    }
}