using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreLib.Scripting;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(ResolverLink<>), true)]
    public class ResolverLinkDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var resolver = property.FindPropertyRelative("_resolver");
            var path     = property.FindPropertyRelative("_path");
            EditorGUI.LabelField(position.GuiField(0), label);
            EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 0), resolver, GUIContent.none);

            var res = (resolver.objectReferenceValue as Resolver);
            if (res != null)
            {
                var type  = property.GetFieldType().GetGenericArguments().First();
                var paths = _getPathsOfType(type).ToList();
                var index = paths.IndexOf(path.stringValue);
                
                if (index == -1 && path.stringValue.IsNullOrEmpty())
                    index = 0;

                if (index != -1 && paths.IsEmpty() == false)
                {
                    var n = EditorGUI.Popup(position.GuiField(0).Field().WidthSegment(2, 1), index, paths.ToArray());
                    path.stringValue = paths[n];
                }
                else
                {
                    GUI.color = Color.red;
                    EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 1), path, GUIContent.none);
                    GUI.color = Color.white;
                }
                
                return;
            }

            EditorGUI.PropertyField(position.GuiField(0).Field().WidthSegment(2, 1), path, GUIContent.none);

            // -----------------------------------------------------------------------
            IEnumerable<string> _getPathsOfType(Type type)
            {
                try
                {
                    var root = (res._root.Enabled ? res._root.Value.Value : res.gameObject).transform;
                    return root
                           .GetComponentsInChildren(typeof(IRef<>).MakeGenericType(type))
                           .Select(n => ((Component)n).gameObject.GetGameObjectPath(root.gameObject))
                           .Distinct();
                }
                catch
                {
                    return Enumerable.Empty<string>();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lines = 1;
            return lines * EditorGUIUtility.singleLineHeight;
        }
    }
}