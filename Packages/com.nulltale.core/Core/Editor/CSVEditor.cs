using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace CoreLib
{
    [CustomEditor(typeof(CSVImporter))]
    public class CSVEditor : ScriptedImporterEditor
    {
        private       CSVImporter _csvImporter;
        
        // =======================================================================
        public override void OnEnable()
        {
            base.OnEnable();
            
            _csvImporter = (CSVImporter)target;
        }

        public override void OnInspectorGUI()
        {
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CSVImporter._sheetID)), new GUIContent("Sheet"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CSVImporter._gidID)), new GUIContent("Gid"));
			
            
            serializedObject.ApplyModifiedProperties();
            
            ApplyRevertGUI();
        }

		protected override bool OnApplyRevertGUI()
		{
            if (GUILayout.Button("Sync"))
			{
				AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GetAssetPath(_csvImporter)), new []{"CSV"});
#pragma warning disable CS4014
				CSVImporter._syncAsset(_csvImporter.assetPath, _csvImporter._sheetID, _csvImporter._gidID.ToString());
#pragma warning restore CS4014
			}
			
			if (GUILayout.Button("Url"))
				Application.OpenURL(string.Format(CSVImporter.UrlLinkPattern, _csvImporter._sheetID, _csvImporter._gidID));
				
			return base.OnApplyRevertGUI();
		}
	}
}