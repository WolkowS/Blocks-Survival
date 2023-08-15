using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Networking;

namespace CoreLib
{
    [ScriptedImporter(1, "csvg")]
    public class CSVImporter : ScriptedImporter
    {
		public const string UrlCVSPattern  = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
		public const string UrlLinkPattern = "https://docs.google.com/spreadsheets/d/{0}/edit#gid={1}";
		
        public string _sheetID;
        public int    _gidID;
        
        // =======================================================================
        public override void OnImportAsset(AssetImportContext ctx)
        {
			if (_sheetID.IsNullOrEmpty())
				_sheetID = EditorPrefs.GetString("Sheet path");
			
            var asset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", asset);
            
            ctx.SetMainObject(asset);
            
            AssetDatabase.ImportAsset(ctx.assetPath, ImportAssetOptions.ForceUpdate);
        }

        public static async Task _sync()
        {
            var tasks = new List<Task>();
			
            foreach (var cvs in Extensions.FindAssets<TextAsset>("CVS"))
            {
                var assetPath = AssetDatabase.GetAssetPath(cvs);
                var importer  = (CSVImporter)AssetImporter.GetAtPath(assetPath);
                tasks.Add(_syncAsset(assetPath, importer._sheetID, importer._gidID.ToString()));
            }

            await Task.WhenAll(tasks.ToArray());
        }
		
		public static async Task _syncAsset(string assetPath)
		{
			var importer = (CSVImporter)GetAtPath(assetPath);
			await _syncAsset(assetPath, importer._sheetID, importer._gidID.ToString());
		}
		
		public static async Task _syncAsset(string assetPath, string sheetID, string gidID)
        {
			var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
			var url = string.Format(UrlCVSPattern, sheetID, gidID);

            Debug.Log($"Downloading: {Path.GetFileName(assetPath)}...", asset);
			var request = UnityWebRequest.Get(url);
			await request.SendWebRequest();
			
			await File.WriteAllBytesAsync(assetPath, request.downloadHandler.data);
			
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
			Debug.Log($"CVS <color=yellow>{assetPath}</color> successfully saved", asset);
		}
    }
	
	
    public class CSVImporterSettings : SettingsProvider
    {
		// =======================================================================
        private CSVImporterSettings(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public override void OnGUI(string searchContext)
        {
            //EditorGUILayout.ObjectField(null, typeof(AssemblyDefinitionAsset), false);
			EditorPrefs.SetString("Sheet path", EditorGUILayout.TextField("Sheet path", EditorPrefs.GetString("Sheet path")));
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new CSVImporterSettings("Preferences/CSVImporterSettings", SettingsScope.User);

            // Automatically extract all keywords from the Styles.
            //provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
    }
}