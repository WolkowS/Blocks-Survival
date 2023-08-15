using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreLib.StaticInjector;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(StaticInjector), menuName = Core.k_CoreModuleMenu + nameof(StaticInjector))]
    public class StaticInjector : Core.Module<StaticInjector>
    {
        public SoCollection<FieldInjector>    _injections;
        
        // =======================================================================
        public override void Init()
        {
            foreach (var injection in _injections)
                injection.Invoke();
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void _invokeAfterCompilation()
        {
            Core.Instance?.GetModule<StaticInjector>()?.Init();
        }
#endif

        public FieldInjector GetInjector(string assemblyFullName, string typeFullName, string fieldName)
        {
            return _injections.Values.FirstOrDefault(n => n._assembly == assemblyFullName && n._type == typeFullName && n._field == fieldName);
        }
        
        [Button]
        internal void _update()
        {
#if UNITY_EDITOR
            var dirty = false;
            var founded = new List<FieldInjector>();
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.GetCustomAttributes().OfType<StaticInjectionAttribute>().Any() == false)
                    continue;
                
                var injector = GetInjector(assembly.FullName, type.FullName, field.Name);
                if (injector == null)
                {
                    injector = ScriptableObject.CreateInstance<FieldInjector>();
                    injector.name      = $"{type.FullName}.{field.Name}";
                    injector._assembly = assembly.FullName;
                    injector._type     = type.FullName;
                    injector._field    = field.Name;
                    
                    _injections.Add(injector);
                    UnityEditor.AssetDatabase.AddObjectToAsset(injector, this);
                    
                    dirty = true;
                }
                
                founded.Add(injector);
            }
            
            // remove not used fields
            foreach (var injector in _injections.Values.Where(n => founded.Contains(n) == false).ToArray())
                _injections.Remove(injector);
            
            if (dirty)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(this), UnityEditor.ImportAssetOptions.ForceUpdate);
            }
#endif
        }
    }
    
}