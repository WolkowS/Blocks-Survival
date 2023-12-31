using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(SoCollection<>), true)]
    public class SoCollectionDrawer : PropertyDrawer
    {
        private ReorderableList m_List;

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                var list = _getList(property);
                list.DoList(position);
                list.GetHeight();
            }
            catch
            {
                // pass
            }
        }              
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            try
            {
                return _getList(property).GetHeight();
            }
            catch
            {
                return EditorGUI.GetPropertyHeight(property);
            }
        }
        
        // =======================================================================
        private ReorderableList _getList(SerializedProperty property)
        {
            if (m_List == null)
            {
                var propertyList = property.FindPropertyRelative("m_List");
                var collectionType = propertyList.GetSerializedValue<object>().GetType().GetGenericArguments().First();

                // setup types, create list
                var types = new List<Type>();
                if (attribute is SOCollectionTypesAttribute attr)
                    types.AddRange(attr.Types);

                if (types.IsEmpty())
                    types.Add(collectionType);

                types.RemoveAll(n => n.IsAbstract);

                m_List = new ReorderableList(propertyList.serializedObject, propertyList, true, true, true, true);
                m_List.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.width -= 30;
                    rect.x += 30 * 0.5f;

                    var element = propertyList.GetArrayElementAtIndex(index);

                    var hasValue = element.objectReferenceValue != null;
                    var isInner = element.objectReferenceValue != null && element.serializedObject.GetNestedAssets().Contains(element.objectReferenceValue);

                    // ref
                    using (new Utils.DisablingScope(isInner))
                    {
                        EditorGUI.BeginChangeCheck();
                        var picked = EditorGUI.ObjectField(rect.WithHeight(EditorGUIUtility.singleLineHeight), new GUIContent(" "), element.objectReferenceValue, types.FirstOrDefault() ?? collectionType, false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (picked == null || _containtsName(picked.name) == false)
                                element.objectReferenceValue = picked;
                            else
                                Debug.Log($"Can't add object {picked} with duplicated name {picked.name}");
                        }
                    }


                    if (hasValue)
                        element.isExpanded = EditorGUI.Foldout(rect.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(5), element.isExpanded, GUIContent.none, toggleOnLabelClick: true);

                    // name
                    if (isInner)
                    {
                        var elementSO = element.objectReferenceValue as ScriptableObject;
                        if (elementSO != null)
                        {
                            // name must be unique
                            var newName = EditorGUI.DelayedTextField(rect.Label(), elementSO.name);
                            if (newName != elementSO.name && _containtsName(newName) == false)
                            {
                                elementSO.name = newName;
                                DelayedUpdate.Plan(AssetDatabase.SaveAssets);
                            }

                            element.DrawObjectReference(rect);
                        }
                    }
                    else
                    {
                        using (new Utils.DisablingScope())
                            EditorGUI.TextField(rect.Label(), element.objectReferenceValue?.name ?? string.Empty);

                        element.DrawObjectReference(rect);
                    }

                    // ===================================
                    bool _containtsName(string name)
                    {
                        return propertyList.GetList().Where(n => n.objectReferenceValue != null).Any(n => n.objectReferenceValue.name == name);
                    }
                };
                m_List.onRemoveCallback = list =>
                {
                    var element = propertyList.GetArrayElementAtIndex(list.index);

                    var isNested = element.objectReferenceValue != null && element.serializedObject.GetNestedAssets().Contains(element.objectReferenceValue);
                    var hasValue = element.objectReferenceValue != null;
                    var obj      = element.objectReferenceValue;

                    var doNotRemove = Event.current.modifiers == EventModifiers.Shift || isNested == false || hasValue == false 
                                      || propertyList.GetList().Select(n => n.objectReferenceValue).Count(n => n == element.objectReferenceValue) > 1;

                    if (doNotRemove == false)
                    {
                        AssetDatabase.RemoveObjectFromAsset(obj);
                        Object.DestroyImmediate(element.objectReferenceValue);
                        element.objectReferenceValue = null;
                        AssetDatabase.SaveAssets();
                    }

                    var index = list.index;
                    propertyList.DeleteArrayElementAtIndex(index);
                };
                m_List.onAddCallback = list =>
                {
                    // collect options
                    var options   = new List<KeyValuePair<string, Action>>();
                    var shiftHeld = Event.current.modifiers == EventModifiers.Shift;

                    // empty object option
                    if (shiftHeld)
                        options.Add(new KeyValuePair<string, Action>("Add Empty", () =>
                        {
                            propertyList.arraySize++;
                            propertyList.GetArrayElementAtIndex(propertyList.arraySize - 1).objectReferenceValue = null;
                            propertyList.serializedObject.ApplyModifiedProperties();
                        }));

                    // options from types
                    options.AddRange(types.Select(type => new KeyValuePair<string, Action>(type.Name, () => _createElementOfType(type))));

                    // if shift held add from existing and derived
                    if (shiftHeld)
                    {

                        var ofType = types.IsEmpty() ? TypeCache.GetTypesDerivedFrom(collectionType)
                                     .Where(type => type.IsAbstract == false && type.IsGenericTypeDefinition == false)
                                     .Except(collectionType)
                                     .ToArray() : types.ToArray();

                        var subAssets = _getAssets(ofType)
                                         .Except(_getElements(ofType))
                                         .Select(so => new KeyValuePair<string, Action>((string.IsNullOrEmpty(so.name) ? "_" : so.name), () => _addExistingElement(so)))
                                         .OrderBy(n => n.Key)
                                         .ToList();

                        if (subAssets.Count > 0)
                        {
                            if (options.IsEmpty() == false)
                                options.Add(new KeyValuePair<string, Action>(string.Empty, null));
                            options.AddRange(subAssets);
                        }
                        
                    }
                    if (shiftHeld || types.IsEmpty())
                    {
                        var derived = TypeCache.GetTypesDerivedFrom(collectionType)
                                     .Where(type => type.IsAbstract == false && type.IsGenericTypeDefinition == false && type.HasAttribute<SOCollectionIgnoreAttribute>() == false)
                                     .Except(collectionType)
                                     .Select(type => new KeyValuePair<string, Action>(type.Name, () => _createElementOfType(type)))
                                     .ToList();

                        if (derived.Count > 0)
                        {
                            if (options.IsEmpty() == false)
                                options.Add(new KeyValuePair<string, Action>(string.Empty, null));
                            options.AddRange(derived);
                        }
                    }

                    // check real options count
                    if (options.Count == 1)
                    {
                        options.First().Value.Invoke();
                    }
                    else
                    {
                        var menu = new GenericMenu();
                        foreach (var option in options)
                        {
                            if (string.IsNullOrEmpty(option.Key))
                                menu.AddSeparator(string.Empty);
                            else
                                menu.AddItem(new GUIContent(option.Key), false, option.Value.Invoke);
                        }

                        menu.ShowAsContext();
                    }

                    // ===================================
                    void _createElementOfType(object type)
                    {
                        propertyList.arraySize++;

                        // create
                        var element = ScriptableObject.CreateInstance((Type)type);
                        element.name = ObjectNames.NicifyVariableName(element.GetType().Name);
                        AssetDatabase.AddObjectToAsset(element, propertyList.serializedObject.targetObject);
                        AssetDatabase.SaveAssets();
                        propertyList.GetArrayElementAtIndex(propertyList.arraySize - 1).objectReferenceValue = element;

                        propertyList.serializedObject.ApplyModifiedProperties();
                    }

                    void _addExistingElement(object element)
                    {
                        propertyList.arraySize++;
                        propertyList.GetArrayElementAtIndex(propertyList.arraySize - 1).objectReferenceValue = (Object)element;
                        propertyList.serializedObject.ApplyModifiedProperties();
                    }

                    IEnumerable<ScriptableObject> _getElements(Type[] ofType)
                    {
                        var result = new List<ScriptableObject>(propertyList.arraySize);

                        for (var n = 0; n < propertyList.arraySize; n++)
                        {
                            var el = propertyList.GetArrayElementAtIndex(n);
                            if (ofType.Contains(el.objectReferenceValue.GetType()) == false)
                                continue;

                            result.Add(el.objectReferenceValue as ScriptableObject);
                        }

                        return result;
                    }

                    IEnumerable<ScriptableObject> _getAssets(Type[] ofType)
                    {
                        return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(propertyList.serializedObject.targetObject))
                                            .Except(new []{propertyList.serializedObject.targetObject})
                                            .OfType<ScriptableObject>()
                                            .Where(n => ofType.Contains(n.GetType()))
                                            .ToList();
                    }
                };
                m_List.drawHeaderCallback = rect =>
                {
                    var style = EditorStyles.label;
                    if (Event.current.modifiers == EventModifiers.Shift)
                    {
                        style = new GUIStyle(EditorStyles.label);
                        style.normal.textColor = Color.yellow;
                    }
                    EditorGUI.LabelField(rect, new GUIContent(property.displayName, "Hold shift to see all options"), style);
                    
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Sort"), false, () =>
                        {
                            var objects = propertyList.GetList().Select(n => n.objectReferenceValue).OrderBy(n => n.name).ToList();
                            propertyList.SetList(objects);
                            propertyList.serializedObject.ApplyModifiedProperties();
                        });

                        menu.ShowAsContext();
                    }

                };
                m_List.elementHeightCallback = index =>
                {
                    if (propertyList.arraySize == 0)
                        return 0;

                    return propertyList.GetArrayElementAtIndex(index).GetObjectReferenceHeight();
                };
            }

            return m_List;
        }
        
    }
}