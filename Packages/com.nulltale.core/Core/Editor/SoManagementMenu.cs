using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    public static class SoManagementMenu
    {
        //[MenuItem("CONTEXT/ScriptableObject/ChangeType", true)]
        //[MenuItem("CONTEXT/Component/ChangeType", true)]
        public static bool ChangeTypeValidate(MenuCommand menuCommand)
        {
            using var so = new SerializedObject(menuCommand.context);
            if (so.context == null)
                return false;

            var scriptType = (so.FindProperty("m_Script").objectReferenceValue as MonoScript)?.GetClass();
            if (scriptType == null)
                return false;
            
            var rootType = _getRootType(scriptType);
            if (rootType == null)
                return false;

            return true;
        }

        [MenuItem("CONTEXT/ScriptableObject/ChangeType")]
        [MenuItem("CONTEXT/Component/ChangeType")]
        public static void ChangeType(MenuCommand menuCommand)
        {
            var scripts = new List<MonoScript>();

            var       target     = menuCommand.context;
            using var so         = new SerializedObject(target);
            var       scriptType = (so.FindProperty("m_Script").objectReferenceValue as MonoScript)?.GetClass();
            var       rootType   = _getRootType(scriptType);

            foreach (var type in TypeCache.GetTypesDerivedFrom(rootType).Where(n => n.IsAbstract == false && n.IsGenericTypeDefinition == false))
            {
                if (type == scriptType)
                    continue;

                var guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");
                if (guids.IsEmpty())
                    continue;

                var guid = guids.FirstOrDefault();
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                if (script == null)
                    continue;

                scripts.Add(script);
            }

            if (scripts.IsEmpty())
                return;
            
            ObjectPickerWindow.Show(picked =>
                {
                    var script = picked as MonoScript;
                    if (script == null)
                        return;

                    using var so = new SerializedObject(target);
                    var prop = so.FindProperty("m_Script");
                    prop.objectReferenceValue = script;
                    
                    
                    var mainAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));
                    so.ApplyModifiedProperties();
                    if (mainAsset is GameObject prefab)
                        PrefabUtility.SavePrefabAsset(prefab);
                    
                }, null, scripts, 0, script => new GUIContent(script.name), "Change type");
        }

        private static Type _getRootType(Type scriptType)
        {
            var hierarchy = new List<Type>();
            var t = scriptType;
            while (t.BaseType != null)
            {
                t = t.BaseType;
                hierarchy.Add(t);
            }

            var result = hierarchy.LastOrDefault(n => n.IsAbstract); 
            if (result == null)
                result = hierarchy.LastOrDefault(n => n == typeof(ScriptableObject) || n == typeof(Component));
            
            return result;
        }
        
        private static bool _isSubAsset(Object obj)
        {
            return AssetDatabase.IsSubAsset(obj);
        }

        
        [MenuItem("CONTEXT/ScriptableObject/Delete", true)]
        public static bool DeleteValidation(MenuCommand menuCommand)
        {
            if (MenuItems.CurrentEvent().shift == false)
                return false; 
                    
            return _isSubAsset(menuCommand.context);
        }
        
        [MenuItem("CONTEXT/ScriptableObject/Delete", false, -10000)]
        public static void Delete(MenuCommand menuCommand)
        {
            var target = (ScriptableObject)menuCommand.context;
            AssetDatabase.RemoveObjectFromAsset(target);
            Object.DestroyImmediate(target);
            AssetDatabase.SaveAssets();
        }
        
        
        [MenuItem("CONTEXT/ScriptableObject/UnParent", true)]
        public static bool UnParantValidate(MenuCommand menuCommand)
        {
            var target = (ScriptableObject)menuCommand.context;
            return AssetDatabase.IsSubAsset(target);
        }
        
        [MenuItem("CONTEXT/ScriptableObject/UnParent")]
        public static void UnParant(MenuCommand menuCommand)
        {
            var target = (ScriptableObject)menuCommand.context;                
            var path = AssetDatabase.GetAssetPath(target);
            AssetDatabase.RemoveObjectFromAsset(target);
            AssetDatabase.CreateAsset(target, Path.Combine(Path.GetDirectoryName(path), $"{target.name}.asset"));
        }
        
        [MenuItem("CONTEXT/ScriptableObject/Rename")]
        public static void Rename(MenuCommand menuCommand)
        {
            var target = (ScriptableObject)menuCommand.context;                
            
            RenameWindow.Show(target.name, s =>
            {
                using var so = new SerializedObject(target);
                var prop = so.FindProperty("m_Name");
                if (prop.stringValue == s) 
                    return;

                prop.stringValue = s;

                var assetPath = AssetDatabase.GetAssetPath(target);
                var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                so.ApplyModifiedProperties();
                if (mainAsset is GameObject prefab)
                    PrefabUtility.SavePrefabAsset(prefab);
                
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            });
        }
    }
}