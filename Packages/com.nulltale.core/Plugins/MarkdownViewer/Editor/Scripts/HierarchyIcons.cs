using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.RestService;
using UnityEngine;

namespace MG.MDV
{
    [InitializeOnLoad] static class HierarchyIcons
    {
        // add your components and the associated icons here
        static Dictionary<Type, Content> typeIcons = new Dictionary<Type, Content>()
        {
            { typeof(Note), new Content("d_UnityEditor.ConsoleWindow", Color.white) },
            { typeof(Mark), new Content("d_TreeEditor.Material On", Color.white) },
        };

        // cached game object information
        static Dictionary<int, Content> labeledObjects    = new Dictionary<int, Content>();
        static HashSet<int>                unlabeledObjects  = new HashSet<int>();
        static GameObject[]                previousSelection = null; // used to update state on deselect

        public struct Content
        {
            public GUIContent _content;
            public Color      _color;

            public Content(GUIContent content, Color color)
            {
                _content = content;
                _color   = color;
            }

            public Content(string content, Color color)
            {
                _content = EditorGUIUtility.IconContent(content);
                _color   = color;
            }
        }
        
        // =======================================================================
        static HierarchyIcons()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

            // callbacks for when we want to update the object GUI state:
            ObjectFactory.componentWasAdded += c => UpdateObject(c.gameObject.GetInstanceID());
            // there's no componentWasRemoved callback, but we can use selection as a proxy:
            Selection.selectionChanged += OnSelectionChanged;
        }

        static void OnHierarchyGUI(int id, Rect rect)
        {
            if (unlabeledObjects.Contains(id)) 
                return; // don't draw t$$anonymous$$ngs with no component of interest
            
            if (ShouldDrawObject(id, out var content))
            {
                // GUI code here
                rect.xMin = rect.xMin - 30; // right-align the icon
                GUI.color = content._color;
                GUI.Label(rect, content._content);
                GUI.color = Color.white;
            }
        }

        static bool ShouldDrawObject(int id, out Content icon)
        {
            if (labeledObjects.TryGetValue(id, out icon)) 
                return true;
            
            // object is unsorted, add it and get icon, if applicable
            return SortObject(id, out icon);
        }

        static bool SortObject(int id, out Content icon)
        {
            var go = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (go != null)
            {
                foreach (var (type, content) in typeIcons)
                {
                    var com = go.GetComponent(type);
                    
                    if (com != null)
                    {
                        if (com is Mark marker)
                        {
                            if (marker._self == false)
                                continue;
                                
                            var label = new Content(content._content, marker._color); 
                            labeledObjects.Add(id, label);
                        }
                        else
                        if (com is Note note)
                        {
                            var view = note._view switch
                            {
                                Note.View.Note     => EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow"),
                                Note.View.CheckBox => EditorGUIUtility.IconContent("d_Toggle Icon"),
                                Note.View.Cross    => EditorGUIUtility.IconContent("CrossIcon"),
                                Note.View.Star     => EditorGUIUtility.IconContent("d_Favorite"),
                                Note.View.None     => GUIContent.none,
                                _                  => throw new ArgumentOutOfRangeException()
                            };
                            
                            var label = new Content(view, Color.white);
                            
                            var mark = _getMarker();
                            if (mark != null)
                            {
                                label._color = mark._color;
                                labeledObjects.Add(id, label);
                            }
                            else
                            {
                                labeledObjects.Add(id, content);
                            }

                            // -----------------------------------------------------------------------
                            Mark _getMarker()
                            {
                                var result = go.GetComponent<Mark>();
                                if (result != null && result._self)
                                    return result;
                                
                                var current = go;
                                while (current != null && current.transform.parent != null)
                                {
                                    result = current.transform.parent.GetComponentInParent<Mark>(true);
                                    
                                    if (result == null)
                                        return null;
                                    
                                    if (result._childern)
                                        return result;
                                    
                                    current = result.gameObject;
                                }
                                
                                return null;
                            }
                        }
                        
                        icon = content;
                        return true;
                    }
                }
            }

            unlabeledObjects.Add(id);
            icon = default;
            return false;
        }

        static void UpdateObject(int id)
        {
            unlabeledObjects.Remove(id);
            labeledObjects.Remove(id);
            SortObject(id, out _);
        }

        const int MAX_SELECTION_UPDATE_COUNT = 3; // how many objects we want to allow to get updated on select/deselect

        static void OnSelectionChanged()
        {
            TryUpdateObjects(previousSelection);                         // update on deselect
            TryUpdateObjects(previousSelection = Selection.gameObjects); // update on select
        }

        static void TryUpdateObjects(GameObject[] objects)
        {
            if (objects != null && objects.Length > 0 && objects.Length <= MAX_SELECTION_UPDATE_COUNT)
            { // max of three to prevent performance $$anonymous$$tches when selecting many objects
                foreach (GameObject go in objects)
                {
                    UpdateObject(go.GetInstanceID());
                }
            }
        }
    }
}