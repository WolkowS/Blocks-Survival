using System;
using System.Globalization;
using System.Linq;
using Cinemachine;
using CoreLib.Scripting;
using Malee;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoreLib
{
    [DisallowMultipleComponent, DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public sealed class Core : MonoBehaviour
    {
        public const           string             k_CoreModuleMenu               = "Core Module/";

        public static readonly WaitForFixedUpdate k_WaitForFixedUpdate           = new WaitForFixedUpdate();
        public static readonly WaitForEndOfFrame  k_WaitForEndOfFrame            = new WaitForEndOfFrame();
        public const           int                k_ManagerDefaultExecutionOrder = -100;
        public static readonly NumberFormatInfo   k_NumberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };

        public static Camera Camera
        {
            get => Instance.m_Camera;
            set => Instance._deployCamera(value);
        }

        private static Core s_Instance;
        public static Core Instance
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false || s_Instance == null)
                    s_Instance = FindObjectOfType<Core>();
#endif
                return s_Instance;
            }
        }

        [SerializeField]
        private Camera				        m_Camera;
        private CinemachineBrain	        m_CameraBrain;
        public CinemachineBrain			    CameraBrain => m_CameraBrain;

        public bool						    m_DoNotDestroyOnLoad = true;
        
        [SerializeField]
        private ThreadPriority              m_LoadingPriority;
        [Tooltip("Unload scenes without MainScene go")]
        public bool                         m_MainScenes;
        [Tooltip("Apply object states before play")]
        public bool                         m_ObjectStates;
        [Tooltip("Save scenes before play")]
        public bool                         m_SaveScenes;

        [SerializeField]
        private Optional<int> m_Fps = new Optional<int>(60, false);
        [SerializeField]
        private Optional<Vector3> m_SortingAxis = new Optional<Vector3>(Vector3.up, false);
        [SerializeField]
        internal Optional<GameObject> _pointer;
        public int Fps
        {
            get => m_Fps.Value;
            set
            {
                Application.targetFrameRate = value;
                m_Fps.Value = value;
                m_Fps.Enabled = true;
            }
        }
	
        [SerializeField] [Tooltip("Log filter for release mode")]
        internal Optional<LogType>   _runtimeLogs = new Optional<LogType>(LogType.Error, true);
	
        [SerializeField, Reorderable(elementNameProperty = "m_Module", surrogateType = typeof(Module), surrogateProperty = "m_Module")]
        private ModuleList                  m_Modules;

        [NonSerialized]
        public Scene m_BufferScene;
        
        
        // =======================================================================
        public interface IModule
        {
            void Init();
        }

        public abstract class Module : ScriptableObject, IModule
        {
            public abstract void Init();
        }

        public abstract class Module<TSingleton> : Module
            where TSingleton: Module
        {
            protected static TSingleton s_Instance;
            public static TSingleton Instance
            {
                get
                {
#if UNITY_EDITOR
                    // for unity editor tools
                    if (s_Instance == null && Application.isEditor && Application.isPlaying == false)
                        Instance = Resources.FindObjectsOfTypeAll<TSingleton>().FirstOrDefault();
#endif
                    return s_Instance;
                }

                private set => s_Instance = value;
            }
        }

        [Serializable]
        public class ModuleWrapper
        {
            [SerializeField]
            private bool                    m_Active;
            [SerializeField]
            [Expandable]
            private UnityEngine.Object      m_Module;

            private IModule                 m_Instance;

            public IModule                  Module => m_Active ? m_Instance : null;
            internal UnityEngine.Object     ModuleObject => m_Active ? m_Module : null;

            // =======================================================================
            public void Init()
            {
                if (m_Active == false)
                    return;

                if (m_Module == null)
                    return;

                switch (m_Module)
                {
                    case GameObject go:
                    {
                        // instantiate if instance not a scene object
                        m_Instance = go.gameObject.scene.name == null ? (Instantiate(go, Core.Instance.transform) as GameObject).GetComponent<IModule>() : go.GetComponent<IModule>();
                    } break;

                    case ScriptableObject so:
                        m_Instance = m_Module as IModule;
                        break;

                    default:
                        m_Instance = null;
                        break;
                }

                if (m_Instance != null)
                {
                    // set singleton instance
                    var mod = m_Instance.GetType().GetBaseTypes()
                                        .FirstOrDefault(n => n.IsGenericType &&
                                                             n.GetGenericTypeDefinition() == typeof(Module<>));
                    if (mod != null)
                    {
                        var prop = mod.GetProperty("Instance");
                        prop.SetValue(m_Instance, m_Instance);
                    }

                    // init module
                    m_Instance.Init();
                }
            }
        }

        [Serializable]
        public class ModuleList : ReorderableArray<ModuleWrapper> {}

        [Serializable]
        public enum ProjectSpace
        {
            XY,
            XZ
        }

        // =======================================================================
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void _editorFunc()
        {
            UnityEditor.EditorApplication.playModeStateChanged += state =>
            {
                if (state != UnityEditor.PlayModeStateChange.ExitingEditMode)
                    return;
                
                var core = FindObjectOfType<Core>();
                if (core == null)
                    return;
                
                if (core.m_SaveScenes)
                    UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            };
        }
#endif
        
        private void Awake()
        {
#if UNITY_EDITOR
            if (m_ObjectStates)
            {
                foreach (var objectState in FindObjectsOfType<ObjectState>(true))
                    objectState.ApplyRuntime();
                
                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    foreach (var objectState in FindObjectsOfType<ObjectState>(true))
                        objectState.ApplyRuntime();
                };
            }
            
            if (m_MainScenes)
            {
                var sceneList = FindObjectsOfType<MainScene>(false)
                                .Select(n => n.gameObject.scene)
                                .Append(gameObject.scene)
                                .Append(SceneManager.GetSceneByName("DontDestroyOnLoad"))
                                .Distinct()
                                .ToList();

                for (var n = 0; n < SceneManager.sceneCount; n++)
                {
                    var scene = SceneManager.GetSceneAt(n);
                    if (sceneList.Contains(scene))
                        continue;
                    
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
#endif
            
#if !UNITY_EDITOR
            if (_runtimeLogs.Enabled && Debug.isDebugBuild == false)
                Debug.unityLogger.filterLogType = _runtimeLogs.Value;
#endif
            if (SceneManager.GetActiveScene() != gameObject.scene)
                Debug.LogError("Core is not a main scene!");

            Application.backgroundLoadingPriority = m_LoadingPriority;
            
            // set instance
            s_Instance = this;

            // create buffer scene
            m_BufferScene = UnityEngine.SceneManagement.SceneManager.CreateScene(" ");

            _deployCamera(m_Camera);
            
            // init modules
            foreach (var module in m_Modules)
                module.Init();
            
            // set target fps
            if (m_Fps.Enabled)
                Application.targetFrameRate = m_Fps.Value;
        }

        private void Start()
        {
            // move to DontDestroyOnLoad
            if (m_DoNotDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void Update() 
        {
#if UNITY_EDITOR
            if (UnityEngine.InputSystem.Keyboard.current.pauseKey.wasPressedThisFrame)
                Debug.Break();
#endif
        }

        private void OnValidate()
        {
            if (m_SortingAxis.Enabled)
            {
                m_Camera.transparencySortAxis = m_SortingAxis.Value;
                m_Camera.transparencySortMode = TransparencySortMode.CustomAxis;
            }
                
            // set target fps
            if (m_Fps.Enabled)
                Application.targetFrameRate = m_Fps.Value;
        }

        public static void Log(string text)
        {
            Debug.Log(text);
        }

        public IModule GetModule(Func<IModule, bool> check)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return m_Modules.FirstOrDefault(n => check(n.Module))?.Module;
            else
                return m_Modules.FirstOrDefault(n => check(n.ModuleObject as IModule))?.ModuleObject as IModule;

#else
            foreach (var moduleWrapper in m_Modules)
            {
                if (check(moduleWrapper.Module))
                    return moduleWrapper.Module;
            }

            return default;
#endif
        }

        public T GetModule<T>() where T : class, IModule
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return m_Modules.FirstOrDefault(n => n.Module is T)?.Module as T;
            else
                return m_Modules.FirstOrDefault(n => n.ModuleObject is T)?.ModuleObject as T;
#else
            foreach (var moduleWrapper in m_Modules)
            {
                if (moduleWrapper.Module is T result)
                    return result;
            }

            return default;
#endif
        }
        
        // =======================================================================
        private void _deployCamera(Camera cam)
        { 
            if (cam == null)
                return;
            
            m_Camera      = cam;
            m_CameraBrain = cam.GetComponent<CinemachineBrain>();
            
            if (m_SortingAxis.Enabled)
            {
                m_Camera.transparencySortAxis = m_SortingAxis.Value;
                m_Camera.transparencySortMode = TransparencySortMode.CustomAxis;
            }
        }
    }
}
