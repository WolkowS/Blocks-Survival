using System;
using System.Collections.Generic;
using CoreLib.Events;
using CoreLib.Module;
using CoreLib.Sound;
using CoreLib.States;
using CoreLib.Values;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(GlobalValue), true)]
    [CustomPropertyDrawer(typeof(PlayerPrefs), true)]
    [CustomPropertyDrawer(typeof(GlobalStateBase), true)]
    [CustomPropertyDrawer(typeof(IdAsset), true)]
    [CustomPropertyDrawer(typeof(SoundAsset), true)]
    [CustomPropertyDrawer(typeof(GlobalEvent), true)]
    [CustomPropertyDrawer(typeof(PlayableAsset), true)]
    public class SoFactoryDrawer : PropertyDrawer
    {
        private static Factory[] s_Factories = new Factory[]
        {
            new PlayerPrefsFactory(),
            new AnyFactory(),
        };

        // =======================================================================
        public abstract class Factory
        {
            public abstract bool TryCreate(Type fieldType, string name, SerializedProperty prop);
        }

        public class PlayerPrefsFactory : Factory
        {
            public override bool TryCreate(Type fieldType, string name, SerializedProperty prop)
            {
                if (fieldType.Implements<PlayerPrefs>() == false)
                    return false;

                var playerPrefs = Object.FindObjectOfType<Core>().GetModule<Module.PlayerPrefsValues>();
                if (playerPrefs == null)
                {
                    Debug.LogWarning($"Core doesn't contains PlayerPrefs Module");
                    return false;
                }

                CreateSubAsset(fieldType, name, prop, playerPrefs, Module.PlayerPrefsValues.k_FieldNamePrefsList);

                return true;
            }
        }

        public class AnyFactory : Factory
        {
            public override bool TryCreate(Type fieldType, string name, SerializedProperty prop)
            {
                if (_isCreatable(fieldType) == false)
                    return false;

                var element = ScriptableObject.CreateInstance(_getType(fieldType));
                element.name = name;
                var path = SoCreator.SoCreator.GetTypeFolder(fieldType);
                if (path.IsNullOrEmpty())
                    path = "Assets";
                
                AssetDatabase.CreateAsset(element, $"{path}\\{element.name}.asset");

                prop.objectReferenceValue = element;
                prop.serializedObject.ApplyModifiedProperties();

                return true;

                // -----------------------------------------------------------------------
                Type _getType(Type type)
                {
                    if (type == typeof(GlobalStateBase))
                        return typeof(Gs);
                    
                    return type;
                }
                bool _isCreatable(Type type)
                {
                    if (type == typeof(GlobalStateBase))
                        return true;
                            
                    if (type.IsAbstract)
                        return false;

                    if (type.IsGenericTypeDefinition)
                        return false;

                    return true;
                }
            }
        }

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position.WithHeight(EditorGUIUtility.singleLineHeight), property, label);
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(5), property.isExpanded, GUIContent.none, toggleOnLabelClick: true);
                property.DrawObjectReference(position);
            }
            
            // ping ref on left click or play audio
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 0 && property.objectReferenceValue != null)
            {
                EditorGUIUtility.PingObject((Object)property.objectReferenceValue);
            }

            // create instance
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 1 && property.objectReferenceValue == null)
            {
                var fieldType = property.GetFieldType();

                var context = new GenericMenu();
                var options = new List<(string, Action)>();

                options.Add(new ("Create", () =>
                {
                    RenameWindow.Show(GetAssetName(property), name =>
                    {
                        if (Event.current.shift)
                        {
                            s_Factories.FirstOfType<AnyFactory>().TryCreate(fieldType, name, property);
                        }
                        else
                        {
                            foreach (var factory in s_Factories)
                            {
                                if (factory.TryCreate(fieldType, name, property))
                                    break;
                            }
                        }
                        
                        if (property.objectReferenceValue != null)
                            Debug.Log($"Asset {property.objectReferenceValue.name} was created at {AssetDatabase.GetAssetPath(property.objectReferenceValue)}", property.objectReferenceValue);
                    });
                }));
                
                foreach (var option in options)
                    context.AddItem(new GUIContent(option.Item1), false, option.Item2.Invoke);

                // show menu
                if (options.IsEmpty() == false)
                    context.ShowAsContext();
                
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetObjectReferenceHeight();
        }

        public static void DrawGlobalValue(Rect position, SerializedProperty property, GUIContent label)
        {
            var isSet = property.objectReferenceValue != null;
            if (isSet)
            {
                EditorGUI.PropertyField(position.WithWidth(EditorGUIUtility.labelWidth).WithHeight(EditorGUIUtility.singleLineHeight), property, GUIContent.none, true);
                if (property.objectReferenceValue != null)
                    using (var so = new SerializedObject(property.objectReferenceValue))
                    {
                        var val = so.FindProperty("m_Value");
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(position, val, new GUIContent(" "), true);
                        if (EditorGUI.EndChangeCheck())
                            so.ApplyModifiedProperties();
                    }

            }
            else
            {
                EditorGUI.PropertyField(position.WithHeight(EditorGUIUtility.singleLineHeight), property, label, true);
            }
        }

        public static float GetGlobalValueHeight(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                using (var so = new SerializedObject(property.objectReferenceValue))
                    return EditorGUI.GetPropertyHeight(so.FindProperty("m_Value"), true);
            }

            return EditorGUIUtility.singleLineHeight;
        }
        
        public static void CreateSubAsset(Type fieldType, string name, SerializedProperty property, Object soTarget, string soCollectionPath)
        {
            using var so = new SerializedObject(soTarget);

            var objectList = so.FindProperty(soCollectionPath).FindPropertyRelative("m_List");

            objectList.arraySize ++;
            
            var element = ScriptableObject.CreateInstance(fieldType);
            element.name = name;
            AssetDatabase.AddObjectToAsset(element, so.targetObject);
            AssetDatabase.SaveAssets();
            objectList.GetArrayElementAtIndex(objectList.arraySize - 1).objectReferenceValue = element;

            so.ApplyModifiedProperties();

            property.objectReferenceValue = element;
            property.serializedObject.ApplyModifiedProperties();
        }

        public static string GetAssetName(SerializedProperty property)
        {
            return $"{property.serializedObject.targetObject.name}.{property.displayName}";
        }
    }
      // Custom property drawer for ADSR envelope parameters
}