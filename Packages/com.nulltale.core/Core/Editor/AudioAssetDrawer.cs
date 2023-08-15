using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Sound;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(SoundAsset), true)]
    public class AudioAssetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position.WithHeight(EditorGUIUtility.singleLineHeight), property, label);
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(5), property.isExpanded, GUIContent.none, toggleOnLabelClick: true);
                property.DrawObjectReference(position);
            }

            if (Event.current.type == EventType.MouseDown)
                Utils.StopAllClips();
            
            // play audio on right click
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 0 && property.objectReferenceValue != null)
            {
                try
                {
                    var clip = (property.objectReferenceValue as SoundAsset)?.Audio.Clip;
                    if (clip != null)
                        Utils.PlayClip(clip);
                }
                catch
                {
                    // ignored
                }
            }

            // create instance
            /*if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 1)
            {
                var soundManager = Object.FindObjectOfType<SoundManager>();
                if (soundManager == null)
                {
                    Debug.LogWarning($"Core doesn't contains SoundManager Module");
                    return;
                }

                var fieldType = property.GetFieldType();

                var context = new GenericMenu();
                var options = new List<(string, Action)>();

                // change type option
                if (property.objectReferenceValue != null && SOChangeTypeMenu.ChangeTypeValidate(new MenuCommand(property.objectReferenceValue)))
                    options.Add(new ("Change Type", () =>
                    {
                        SOChangeTypeMenu.ChangeType(new MenuCommand(property.objectReferenceValue));
                    }));

                // detailed create
                foreach (var type in TypeCache.GetTypesDerivedFrom(fieldType).Where(n => n.IsAbstract == false && n.IsGenericTypeDefinition == false))
                {
                    foreach (var database in soundManager.AudioDatabases)
                    {
                        var path = soundManager.AudioDatabases.Count() > 1 ? $"Create Custom/{database.name}/{type.Name}" : $"Create Custom/{type.Name}";
                        options.Add(new(path, () =>
                        {
                            SOFactoryDrawer.CreateSubAsset(type, property, database, AudioDatabase.k_FieldNameAudioLisst);
                        }));
                    }
                }

                // fast create
                var fastType = _getCreationType();
                if (fastType != null)
                    options.Add(new ("Create", () =>
                    {
                        var mainDatabase = soundManager.AudioDatabases.FirstOrDefault(n => n.Prefix.IsNullOrEmpty());
                        if (mainDatabase == null)
                            mainDatabase = soundManager.AudioDatabases.FirstOrDefault();

                        SOFactoryDrawer.CreateSubAsset(fastType, property, mainDatabase, AudioDatabase.k_FieldNameAudioLisst);
                    }));

                foreach (var option in options)
                    context.AddItem(new GUIContent(option.Item1), false, option.Item2.Invoke);

                // show menu
                if (options.IsEmpty() == false)
                    context.ShowAsContext();

                // -----------------------------------------------------------------------
                Type _getCreationType()
                {
                    if (fieldType == typeof(AudioAsset))
                        return typeof(AudioSound);

                    if (fieldType.IsAbstract || fieldType.IsGenericTypeDefinition)
                        return null;

                    return fieldType;
                }
            }*/
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetObjectReferenceHeight();
        }
    }
}