using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using System.IO;
using CoreLib.SceneManagement;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.ProjectWindowCallback;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib
{
    public static class MenuItems
    {
        private const string k_DefaultDelimiterObjectName = "----------//----------";
        
#if UNITY_EDITOR
        // =======================================================================
        [Shortcut("Create Delimiter")]
        private static void CreateCustomGameObject()
        {
            var selected = UnityEditor.Selection.gameObjects.FirstOrDefault()?.transform;

            var go = new GameObject(k_DefaultDelimiterObjectName);
            go.tag      = "EditorOnly";
            go.isStatic = true;
            go.SetActive(false);
            if (selected != null)
            {
                go.transform.SetParent(selected);
            }

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            UnityEditor.Selection.activeObject = go;
        }
        
        [MenuItem("GameObject/----------||----------", false, 0)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            var selected = UnityEditor.Selection.gameObjects.FirstOrDefault();

            var go = new GameObject(k_DefaultDelimiterObjectName);
            go.tag      = "EditorOnly";
            go.isStatic = true;
            go.SetActive(false);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, selected?.transform.parent?.gameObject);
            if (selected != null)
                go.transform.SetSiblingIndex(selected.transform.GetSiblingIndex() + 1);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            UnityEditor.Selection.activeObject = go;
        }

        [MenuItem("GameObject/Sort", false, 0)]
        private static void SortAlphabetical(MenuCommand menuCommand)
        {
            var go    = menuCommand.context as GameObject;
            var array = go.transform.GetChildren().OrderBy(n => n.name).ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var child = array[index];
                child.SetSiblingIndex(index);
            }
        }
        
        [MenuItem("GameObject/Kill Children", false, 0)]
        private static void KillChildren(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;
            foreach (var child in UnityEditor.Selection.objects.OfType<GameObject>().SelectMany(n => n.GetChildren()).Select(n => n.gameObject).ToArray())
                Object.DestroyImmediate(child);
        }

        [MenuItem("GameObject/Rename Order", false, 0)]
        private static void RenameOrder(MenuCommand menuCommand)
        {
            var pattern = @"_(\d+)";

            var go       = menuCommand.context as GameObject;
            var goup = (UnityEditor.Selection.count == 1 ?
                go.transform.GetChildren().Select(n => n.gameObject) :
                UnityEditor.Selection.objects.OfType<GameObject>().OrderBy(n => n.transform.GetAbsoluteSiblingWeight()))
                           .ToArray();
            for (var n = 0; n < goup.Length; n++)
            {
                var child   = goup[n];
                var replace = $"_{n}";
                if (Regex.IsMatch(child.name, pattern))
                {
                    var result = Regex.Replace(child.name, pattern, replace);
                    child.name = result;
                }
                else
                {
                    child.name += replace;
                }
            }
        }
        
        [MenuItem("GameObject/Restore object states", false, 0)]
        private static void RestoreObjectsStates(MenuCommand menuCommand)
        {
            var go = menuCommand.context as GameObject;
            foreach (var state in go.GetComponentsInChildren<ObjectState>())
            {
                state.Apply();
                state.SetDirty();
            }
            
            Debug.Log("Object states saved and updated");
        }
        
        [MenuItem("Restore object states All", false, 0)]
        private static void RestoreObjectsStatesGlobal()
        {
            foreach (var state in Object.FindObjectsOfType<ObjectState>())
            {
                state.Apply();
                state.SetDirty();
            }
            
            Debug.Log("Object states saved and updated");
        }
        
        [MenuItem("GameObject/Remove Missing Scripts Recursively", false, 0)]
        private static void FindAndRemoveMissingInSelected()
        {
            var deepSelection = EditorUtility.CollectDeepHierarchy(UnityEditor.Selection.gameObjects.OfType<UnityEngine.Object>().ToArray());
            var compCount = 0;
            var goCount = 0;
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {
                    var count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (count > 0)
                    {
                        // Edit: use undo record object, since undo destroy wont work with missing
                        Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        Debug.Log($"Missing script removed from {o.name}", o);
                        compCount += count;
                        goCount++;
                    }
                }
            }
        }

        [MenuItem("CONTEXT/SceneAsset/Create Scene Preset")]
        public static void CreateScenePreset(MenuCommand menuCommand)
        {
            var sceneManager = Object.FindObjectOfType<Core>()?.GetModule<SceneManager>();
            if  (sceneManager == null)
            {
                Debug.LogError("Can't get access to the Core.SceneManager module");
                return;
            }

            var scenes = sceneManager.m_Scenes;
            var asset = scenes.Values.FirstOrDefault(n => n.SceneName == menuCommand.context.name);
            if (asset != null)
            {
                Debug.LogError($"SceneManager already contains scene asset {asset.name} with this scene name");
                return;
            }


            using var so = new SerializedObject(sceneManager);

            var objectList   = so.FindProperty(nameof(SceneManager.m_Scenes)).FindPropertyRelative("m_List");

            objectList.arraySize ++;
            
            var element = ScriptableObject.CreateInstance<ScenePreset>();
            element.name = menuCommand.context.name;
            element.m_SceneName = menuCommand.context.name;
            AssetDatabase.AddObjectToAsset(element, so.targetObject);
            AssetDatabase.SaveAssets();
            objectList.GetArrayElementAtIndex(objectList.arraySize - 1).objectReferenceValue = element;

            so.ApplyModifiedProperties();

            var completeMessage = $"Scene preset <color=yellow>{menuCommand.context.name}</color> was successfully created in {sceneManager.name}.";

            // add scene to the build
            var scenePath = AssetDatabase.GetAssetPath(menuCommand.context);
            if (EditorBuildSettings.scenes.Any(n => n.path == scenePath) == false)
            {
                EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(new EditorBuildSettingsScene(scenePath, true)).ToArray();
                completeMessage += $" The Scene has been added in build.";
            }

            Debug.Log(completeMessage, element);
        }
	
        [MenuItem("Edit/Reserialize Assets", false, 10)]
        private static void ReserializeAssets(MenuCommand menuCommand)
        {
            AssetDatabase.ForceReserializeAssets();
        }

        [MenuItem("Edit/Reload Scripts", false, 10)]
        private static void ReloadScripts(MenuCommand menuCommand)
        {
            EditorUtility.RequestScriptReload();
        }
        
        [Shortcut("Reload Scripts", KeyCode.R, ShortcutModifiers.Control)]
        private static void ReloadScripts()
        {
            EditorUtility.RequestScriptReload();
        }
		
        [Shortcut("Create Texture", KeyCode.O, ShortcutModifiers.Shift)]
        public static void CreateTexture()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                Extensions.CreateAsset(pathName =>
                {
                    var path = Path.ChangeExtension(pathName, ".png");
                    var tex  = new Texture2D(16, 16);
                    
                    for (var x = 0; x < tex.width; x++)
                    for (var y = 0; y < tex.height; y++)
                        tex.SetPixel(x, y, Color.clear);
                    
                    tex.Apply();
                    
                    File.WriteAllBytes(path, tex.EncodeToPNG());
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                }),
                "Texture",
                (EditorGUIUtility.IconContent("Texture2D Icon").image as Texture2D),
                string.Empty);
        }
        
        [Shortcut("Toggle game object", KeyCode.E, ShortcutModifiers.Control)]
        private static void ToggleGameObject()
        {
            foreach (var go in UnityEditor.Selection.gameObjects)
            {
                Undo.RecordObject(go, "Toggle enable");
                go.SetActive(!go.activeSelf);
            }
        }
        
        [Shortcut("Toggle inspector lock", KeyCode.W, ShortcutModifiers.Control)]
         public static void ToggleInspectorLock()
         {
             ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
             ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
         
        [Shortcut("Ping selected", KeyCode.Q, ShortcutModifiers.Control)]
        private static void PingCurrent()
        {
            EditorGUIUtility.PingObject(ActiveEditorTracker.sharedTracker.activeEditors.FirstOrDefault()?.target);
        }
        
        [MenuItem("CONTEXT/ScriptableObject/Ping")]
        private static void Ping(MenuCommand menuCommand)
        {
            EditorGUIUtility.PingObject(menuCommand.context);
        }
         
        public static Event CurrentEvent()
        {
            var field = typeof(Event).GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic);
            
            if (field != null && field.GetValue(null) is Event current)
                return current;
            
            return null;
        }
        
        [MenuItem("CONTEXT/PlayableDirector/Create Asset")]
        public static void CreateAsset(MenuCommand command)
        {
            var director = (PlayableDirector)command.context;
            director.playableAsset = ScriptableObject.CreateInstance<TimelineAsset>();
        }

        [MenuItem("CONTEXT/PlayableDirector/Save Asset", true)]
        public static bool SaveAssetValidate(MenuCommand command)
        {
            var director = (PlayableDirector)command.context;
            if (AssetDatabase.Contains(director))
                return false;
            
            return true;
        }
        
        [MenuItem("CONTEXT/PlayableDirector/Save Asset")]
        public static void SaveAsset(MenuCommand command)
        {
            var director = (PlayableDirector)command.context;
            var path = EditorUtility.SaveFilePanel("Save timeline", "Assets", director.gameObject.name, "playable");
            if (path.IsNullOrEmpty())
                return;
            
            if (path.StartsWith(Application.dataPath))
                path = "Assets" + path.Substring(Application.dataPath.Length);
            
            var asset = Object.Instantiate(director.playableAsset);
            asset.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            
            director.playableAsset = asset;
        }
        
        [MenuItem("CONTEXT/Object/Invoke")]
        public static void Invoke(MenuCommand command)
        {
            var obj = (Object)command.context;
            var method = obj
                         .GetType()
                         .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         .FirstOrDefault(n => n.Name == "Invoke" && n.GetParameters().Length == 0);
            if (method == null)
            {
                Debug.Log($"Object {obj.GetType().Name} doesn't have Invoke method");
                return;
            }
            
            method.Invoke(obj, new object[]{});
        }
        
        [MenuItem("CONTEXT/PlayableDirector/Play")]
        public static void Play(MenuCommand command)
        {
            var director = (PlayableDirector)(Object)command.context;
            director.Play();
        }
#endif
    }
}