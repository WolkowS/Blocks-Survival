using System;
using System.IO;
using System.Linq;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreLib.FxTexture
{
    [CustomEditor(typeof(FxTextureAsset))]
    public class FxTextureEditor : Editor
    {
        private Editor          _editor;
        private ReorderableList _passes;
        private FxTextureAsset  _asset;

        // =======================================================================
        private void OnEnable()
        {
            _editor = CreateEditor(target, typeof(NaughtyInspector));
            
            var listProp = serializedObject.FindProperty(nameof(FxTextureAsset._passes));
            _passes = new ReorderableList(serializedObject, listProp, true, true, true, true);
            
            _asset = (FxTextureAsset)target;
            _passes.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Passes");
            _passes.elementHeightCallback = index =>
            {
                var element = listProp.GetArrayElementAtIndex(index);
                return element.objectReferenceValue.GetObjectReferenceHeight(element.isExpanded, prop => prop.name != nameof(FxTextureAsset.PassCombine._active));
            };
            _passes.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = listProp.GetArrayElementAtIndex(index);
                
                using var so = new SerializedObject(element.objectReferenceValue);
                var activeProp = so.FindProperty(nameof(FxTextureAsset.PassCombine._active));
                activeProp.boolValue = EditorGUI.ToggleLeft(rect.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(16), GUIContent.none, activeProp.boolValue);
                so.ApplyModifiedProperties();
                element.objectReferenceValue.DrawObjectReference(element.isExpanded, rect, false, prop => prop.name != nameof(FxTextureAsset.PassCombine._active));
                
                if (GUI.Button(rect.WithHeight(EditorGUIUtility.singleLineHeight).IncX(18).WithWidth(rect.width - 18), new GUIContent(element.objectReferenceValue.GetType().Name), GUI.skin.button))
                    element.isExpanded = !element.isExpanded;
            };
            _passes.onAddCallback = list =>
            {
                var menu = new GenericMenu();
                foreach (var passType in TypeCache.GetTypesDerivedFrom<FxTextureAsset.Pass>().Where(n => n.IsAbstract == false && n.IsGenericTypeDefinition == false))
                {
                    menu.AddItem(new GUIContent(passType.Name), false, () => { _createSubAsset(passType); });
                }
                menu.ShowAsContext();

                // -----------------------------------------------------------------------
                void _createSubAsset(Type passType)
                {
                    var pass = ScriptableObject.CreateInstance(passType);
                    pass.name = passType.Name;
                    AssetDatabase.AddObjectToAsset(pass, target);
                    
                    listProp.arraySize++;
                    listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue = pass;
                    listProp.serializedObject.ApplyModifiedProperties();
                    
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target), ImportAssetOptions.ForceUpdate);
                }
            };
            _passes.onRemoveCallback = list =>
            {
                var pass = listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue;
                AssetDatabase.RemoveObjectFromAsset(pass);
                DestroyImmediate(pass);
                
                listProp.arraySize--;
                listProp.serializedObject.ApplyModifiedProperties();
                
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target), ImportAssetOptions.ForceUpdate);
            };
            
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            _drawProperty(nameof(FxTextureAsset._size));
            _drawProperty(nameof(FxTextureAsset._gradient));
            _drawProperty(nameof(FxTextureAsset._point));

            _passes.DoLayoutList();
            
            if (GUILayout.Button("Bake"))
            {
                if (_asset._texture == null)
                    _asset._bake();

                _save();
            }
            
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                _asset._bake();
                
                if (Event.current.shift)
                    _save();
            }
        }

        private void _save()
        {
            var path  = $"{Path.GetDirectoryName(AssetDatabase.GetAssetPath(_asset))}\\{_asset.name}.png";
            var bytes = _asset._texture.EncodeToPNG();

            File.WriteAllBytes(path, bytes);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
        }

        private void _drawProperty(string propName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propName));
        }
        
        public override bool HasPreviewGUI() => true;

        public override void DrawPreview(Rect previewArea)
        {
            var asset    = ((FxTextureAsset)target);
            var gradient = asset.CreateTexture();
            EditorGUI.DrawTextureTransparent(previewArea, gradient, ScaleMode.ScaleToFit);
        }
    }
}