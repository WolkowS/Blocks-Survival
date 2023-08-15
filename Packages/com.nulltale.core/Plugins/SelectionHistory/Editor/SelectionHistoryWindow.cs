// Staggart Creations http://staggart.xyz
// Copyright protected under Unity asset store EULA

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public class SelectionHistoryWindow : EditorWindow
{
    [MenuItem("Tools/Selection History/Window")]
    public static void Init()
    {
        var window = GetWindow<SelectionHistoryWindow>();
        
        //Options
        window.autoRepaintOnSceneChange = true;
        window.titleContent.image = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneHierarchyWindow" : "UnityEditor.SceneHierarchyWindow").image;
        window.titleContent.text = " Selection History";
        window.wantsMouseMove = true;
        
        //Show
        window.Show();
    }

    [Serializable]
    public class SelectionQueue
    {
        public string       project;
        public int          index;
        public List<string> content = new List<string>();
        private bool        ignoreSelection;
        private int         limit = 50;
        
        // =======================================================================
        public void Add(Object obj)
        {
            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            
            if (obj == null)
                return;
            
            if (content.Count > 0 && (index + 1) != content.Count)
                content.RemoveRange(index + 1, content.Count - (index + 1));
            
            content.Add(GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString());
            if (content.Count > limit)
                content.RemoveRange(0, content.Count - limit);
            
            index = content.Count - 1;
        }

        public void Front() => _select(index + 1);
        public void Back()  => _select(index - 1);
        
        // =======================================================================
        private void _select(int n)
        {
            if (ignoreSelection)
                return;
            
            if (content.Count == 0)
            {
                index = 0;
                return;
            }

            var newIndex = Mathf.Clamp(n, 0, content.Count - 1);
            if (newIndex == index)
                return;
            index = newIndex;
            
            if (GlobalObjectId.TryParse(content[index], out var id) == false)
                return;
            
            var obj =  GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            //if (obj == Selection.activeObject)
            //    return;
            ignoreSelection = true;
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
    private static SelectionQueue s_SelectionQueue;
    
    [InitializeOnLoadMethod]
    private static void InitSelectionQueue()
    {
        if (EditorPrefs.HasKey("SelectionQueue"))
        {
            s_SelectionQueue = JsonUtility.FromJson<SelectionQueue>(EditorPrefs.GetString("SelectionQueue"));
        }
        if (s_SelectionQueue == null || s_SelectionQueue != null && s_SelectionQueue.project != PlayerSettings.productName)
        {
            s_SelectionQueue = new SelectionQueue();
            s_SelectionQueue.project = PlayerSettings.productName;
        }
        
        AssemblyReloadEvents.beforeAssemblyReload += () => EditorPrefs.SetString("SelectionQueue", JsonUtility.ToJson(s_SelectionQueue));
        Selection.selectionChanged                += () => s_SelectionQueue.Add(Selection.activeObject);
    }
    
    [Shortcut("Tools/Selection History/Next",  KeyCode.KeypadPlus, ShortcutModifiers.Control)]
    public static void NextSel()
    {
        s_SelectionQueue.Front();
    }
    
    [Shortcut("Tools/Selection History/Prev",  KeyCode.KeypadMinus, ShortcutModifiers.Control)]
    public static void PrevSel()
    {
        s_SelectionQueue.Back();
    }
    
    //[Shortcut("Tools/Selection History/Next",  KeyCode.KeypadPlus, ShortcutModifiers.Control)]
    public static void Next()
    {
        _data.selectionHistorySeq.RemoveAll(n => n == null);
        _data.selectedIndexSeq = Mathf.Clamp(_data.selectedIndexSeq, 0, _data.selectionHistorySeq.Count - 1);
        var prev = _data.selectionHistorySeq.Count > 0 ? _data.selectionHistorySeq[_data.selectedIndexSeq] : null;
        
        _data.selectedIndexSeq = Mathf.Clamp(_data.selectedIndexSeq + 1, 0, _data.selectionHistorySeq.Count - 1);
        if (_data.selectedIndexSeq < _data.selectionHistorySeq.Count && _data.selectionHistorySeq.Count > 0)
        {
            var obj = _data.selectionHistorySeq[_data.selectedIndexSeq];
            EditorGUIUtility.PingObject(obj);
            if (prev == obj)
                return;
            _muteRecording = true;
            Selection.activeObject = obj;
        }
    }
    
    //[Shortcut("Tools/Selection History/Prev",  KeyCode.KeypadMinus, ShortcutModifiers.Control)]
    public static void Prev()
    {
        _data.selectionHistorySeq.RemoveAll(n => n == null);
        _data.selectedIndexSeq = Mathf.Clamp(_data.selectedIndexSeq, 0, _data.selectionHistorySeq.Count - 1);
        var prev = _data.selectionHistorySeq.Count > 0 ? _data.selectionHistorySeq[_data.selectedIndexSeq] : null;
        
        _data.selectedIndexSeq = Mathf.Clamp(_data.selectedIndexSeq - 1, 0, _data.selectionHistorySeq.Count - 1);
        if (_data.selectedIndexSeq < _data.selectionHistorySeq.Count)
        {
            var obj = _data.selectionHistorySeq[_data.selectedIndexSeq];
            EditorGUIUtility.PingObject(obj);
            if (prev == obj)
                return;
            
            _muteRecording          = true;
            Selection.activeObject = obj;
        }
    }
    
    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        if (EditorPrefs.HasKey("SelectionHistoryData"))
        {
            _data = JsonUtility.FromJson<Data>(EditorPrefs.GetString("SelectionHistoryData"));
        }
        _data ??= new Data();
        
        AssemblyReloadEvents.beforeAssemblyReload += () => EditorPrefs.SetString("SelectionHistoryData", JsonUtility.ToJson(_data));
        Selection.selectionChanged                += SelectionHistoryWindow._selectionChange;
    }

    private string iconPrefix => EditorGUIUtility.isProSkin ? "d_" : "";
    
    public static bool RecordHierarchy
    {
        get => EditorPrefs.GetBool(PlayerSettings.productName + "_SH_RecordHierachy", true);
        set => EditorPrefs.SetBool(PlayerSettings.productName + "_SH_RecordHierachy", value);
    }
    
    public static bool RecordProject
    {
        get => EditorPrefs.GetBool(PlayerSettings.productName + "_SH_RecordProject", true);
        set => EditorPrefs.SetBool(PlayerSettings.productName + "_SH_RecordProject", value);
    }
    
    public static int MaxHistorySize
    {
        get => EditorPrefs.GetInt(PlayerSettings.productName + "_SH_MaxHistorySize", 50);
        set => EditorPrefs.SetInt(PlayerSettings.productName + "_SH_MaxHistorySize", value);
    }
    
    public static bool WriteItemSelection
    {
        get => EditorPrefs.GetBool(PlayerSettings.productName + "_SH_WriteItemSelection", true);
        set => EditorPrefs.SetBool(PlayerSettings.productName + "_SH_WriteItemSelection", value);
    }

    private AnimBool settingAnimation;
    private bool settingExpanded;
    private AnimBool clearAnimation;
    private bool historyVisible = true;


    [Serializable]
    public class Data
    {
        public List<Object> selectionHistory    = new List<Object>();
        public List<Object> selectionHistorySeq = new List<Object>();
        public int          selectedIndex       = -1;
        public int          selectedIndexSeq    = -1;
    }

    private static Data _data;
    private static bool _muteRecording;
    private        bool _hasFocus;
    
    private void OnSelectionChange()
    {
        this.Repaint();
    }
    
    private static void _selectionChange()
    {
        if (_muteRecording)
        {
            _muteRecording = false;
            return;
        }
        
        if (Selection.activeObject == null) 
            return;
        

        _addToHistory();
    }

    private void OnFocus()
    {
        //Items have have been deleted and should be removed from history
        _data.selectionHistory = _data.selectionHistory.Where(x => x != null).ToList();

        _hasFocus = true;
    }
    
    private void OnLostFocus()
    {
        _hasFocus = false;
    }

    private void OnInspectorUpdate() //10 fps
    {
        if (_hasFocus)  Repaint();
    }

    private static void _addToHistory()
    {
        //Skip selected folders and such
        if (Selection.activeObject == null)
            return;
        
        if (Selection.activeObject is ScriptableObject so && AssetDatabase.Contains(so) == false)
            return;
        
        if (Selection.activeObject is DefaultAsset)
            return;

        if (EditorUtility.IsPersistent(Selection.activeObject) && !RecordProject)
            return;
        
        if (EditorUtility.IsPersistent(Selection.activeObject) == false && !RecordHierarchy)
            return;
        
        if(_data.selectionHistory.Contains(Selection.activeObject) == false) 
            _data.selectionHistory.Insert(0, Selection.activeObject);
        
        // Trim end
        if(_data.selectionHistory.Count-1 == MaxHistorySize) 
            _data.selectionHistory.RemoveAt(_data.selectionHistory.Count-1);
        
        // fast trash coding, remove all after index selection
        if (_data.selectionHistorySeq.Count > _data.selectedIndexSeq + 1)
            _data.selectionHistorySeq.RemoveRange(_data.selectedIndexSeq + 1, _data.selectionHistorySeq.Count - 1 - _data.selectedIndexSeq);
        
        _data.selectionHistorySeq.Add(Selection.activeObject);
        if (_data.selectionHistorySeq.Count - 1 == MaxHistorySize)
            _data.selectionHistorySeq.RemoveAt(_data.selectionHistorySeq.Count - 1);
        
        _data.selectedIndexSeq = _data.selectionHistorySeq.Count - 1;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += ListenForNavigationInput;
        
        settingAnimation = new AnimBool(false);
        settingAnimation.valueChanged.AddListener(this.Repaint);
        settingAnimation.speed = 4f;
        clearAnimation = new AnimBool(false);
        clearAnimation.valueChanged.AddListener(this.Repaint);
        clearAnimation.speed = settingAnimation.speed;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= ListenForNavigationInput;
    }
    
    private void ListenForNavigationInput(SceneView sceneView)
    {
        if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.LeftBracket)
        {
            _selectPrevious();
        }
        if (Event.current.type == EventType.KeyDown &&  Event.current.isKey && Event.current.keyCode == KeyCode.RightBracket)
        {
            _selectNext();
        }
    }
    
    private static void _setSelection(Object target, int index)
    {
        _muteRecording = true;
        Selection.activeObject = target;
        EditorGUIUtility.PingObject(target);

        if (WriteItemSelection)
        {
            if (_data.selectionHistorySeq.Count > _data.selectedIndexSeq + 1)
                _data.selectionHistorySeq.RemoveRange(_data.selectedIndexSeq + 1, _data.selectionHistorySeq.Count - 1 - _data.selectedIndexSeq);

            _data.selectionHistorySeq.Add(Selection.activeObject);
            if (_data.selectionHistorySeq.Count - 1 == MaxHistorySize)
                _data.selectionHistorySeq.RemoveAt(_data.selectionHistorySeq.Count - 1);

            _data.selectedIndexSeq = _data.selectionHistorySeq.Count - 1;
        }
    }

    private static void _selectPrevious()
    {
        _data.selectedIndex--;
        _data.selectedIndex = Mathf.Clamp(_data.selectedIndex, 0, _data.selectionHistory.Count - 1);
            
        _setSelection(_data.selectionHistory[_data.selectedIndex], _data.selectedIndex);
    }

    private static void _selectNext()
    {
        _data.selectedIndex++;
        _data.selectedIndex = Mathf.Clamp(_data.selectedIndex, 0, _data.selectionHistory.Count - 1);

        _setSelection(_data.selectionHistory[_data.selectedIndex], _data.selectedIndex);
    }

    private Vector2 scrollPos;

    private void OnGUI()
    {
        _hasFocus = _hasFocus || (Event.current.type == EventType.MouseMove);

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(_data.selectionHistory.Count == 0))
            {
                using (new EditorGUI.DisabledScope(_data.selectedIndex == _data.selectionHistory.Count-1))
                {
                    if (GUILayout.Button(
                        new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "back@2x").image,
                            "Select previous (Left bracket key)"), EditorStyles.miniButtonLeft, GUILayout.Height(20f),
                        GUILayout.Width(30f)))
                    {
                        _selectNext();
                    }
                }

                using (new EditorGUI.DisabledScope(_data.selectedIndex == 0))
                {
                    if (GUILayout.Button(
                        new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "forward@2x").image,
                            "Select next (Right bracket key)"), EditorStyles.miniButtonRight, GUILayout.Height(20),
                        GUILayout.Width(30f)))
                    {
                        _selectPrevious();
                    }
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "TreeEditor.Trash").image, "Clear history"), EditorStyles.miniButton))
                {
                    historyVisible = false;
                }
            }
            
            GUILayout.FlexibleSpace();
            
            settingExpanded = GUILayout.Toggle(settingExpanded, new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "Settings").image, "Edit settings"), EditorStyles.miniButtonMid);
            settingAnimation.target = settingExpanded;
        }
        
        if (EditorGUILayout.BeginFadeGroup(settingAnimation.faded))
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Record", EditorStyles.boldLabel, GUILayout.Width(100f));
                RecordHierarchy = EditorGUILayout.ToggleLeft("Hierarchy", RecordHierarchy, GUILayout.MaxWidth(80f));
                RecordProject = EditorGUILayout.ToggleLeft("Project window", RecordProject, GUILayout.MaxWidth(110f));
                WriteItemSelection = EditorGUILayout.ToggleLeft("Track selection", WriteItemSelection, GUILayout.MaxWidth(110f));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("History size", EditorStyles.boldLabel,GUILayout.Width(100f));
                MaxHistorySize = EditorGUILayout.IntField(MaxHistorySize, GUILayout.MaxWidth(40f));
            }
            
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFadeGroup();
        
        clearAnimation.target = !historyVisible;
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox, GUILayout.MaxHeight(this.maxSize.y-20f));
        {
            EditorGUILayout.BeginFadeGroup(1f-clearAnimation.faded);
            
            var prevColor = GUI.color;
            var prevBgColor = GUI.backgroundColor;

            for (int i = 0; i < _data.selectionHistory.Count; i++)
            {
                if(_data.selectionHistory[i] == null) continue;
                
                var rect = EditorGUILayout.BeginHorizontal();
                
                GUI.color = i % 2 == 0 ?  Color.grey * (EditorGUIUtility.isProSkin ? 1f : 1.7f) : Color.grey * (EditorGUIUtility.isProSkin ? 1.05f : 1.66f);
                
                //Hover color
                if (rect.Contains(Event.current.mousePosition) || Selection.activeObject == (_data.selectionHistory[i]))
                {
                    GUI.color = EditorGUIUtility.isProSkin ? Color.grey * 1.1f : Color.grey * 1.5f;
                }
                
                //Selection outline
                if (Selection.activeObject == (_data.selectionHistory[i]))
                {
                    Rect outline = rect;
                    outline.x -= 1;
                    outline.y -= 1;
                    outline.width += 2;
                    outline.height += 2;
                    EditorGUI.DrawRect(outline, EditorGUIUtility.isProSkin ? Color.gray * 1.5f : Color.gray);
                }

                //Background
                EditorGUI.DrawRect(rect, GUI.color);
                
                GUI.color = prevColor;
                GUI.backgroundColor = prevBgColor;

                if (GUILayout.Button(new GUIContent(" " + _data.selectionHistory[i].name, EditorGUIUtility.ObjectContent(_data.selectionHistory[i], _data.selectionHistory[i].GetType()).image), EditorStyles.label, GUILayout.MaxHeight(17f)))
                {
                    _setSelection(_data.selectionHistory[i], i);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }
        EditorGUILayout.EndScrollView();

        // Once the list is collapse, clear the collection
        if(clearAnimation.faded == 1f)
        {
            _data.selectionHistory.Clear();
            _data.selectionHistorySeq.Clear();
            _muteRecording = false;
        }
        //Reset
        if (_data.selectionHistory.Count == 0) historyVisible = true;
    }
}