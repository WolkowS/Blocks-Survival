using System.IO;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(CSVAttribute))]
    public class CSVDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
         
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == MouseButton.Right)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create"), false, () =>
                {
                    var path     = Path.GetDirectoryName(AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject));
                    var filePath = $"{path}\\{UnityEditor.Selection.activeObject.name}.csvg";
                    
                    var file = System.IO.File.Create(filePath);
                    file.Close();
                    
                    AssetDatabase.ImportAsset(filePath, ImportAssetOptions.Default);
                    AssetDatabase.SaveAssets();
                    var csv = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                    property.objectReferenceValue = csv;
                    property.serializedObject.ApplyModifiedProperties();
                });
                
                menu.ShowAsContext();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}