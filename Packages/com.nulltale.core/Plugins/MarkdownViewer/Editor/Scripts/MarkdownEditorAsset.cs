using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MG.MDV
{
    [CustomEditor(typeof(Note))]
    public class MarkdownEditorAsset : Editor
    {
        private const string k_TextControlId = "Text";
            
        public GUISkin SkinLight;
        public GUISkin SkinDark;

        private MarkdownViewer _viewer;
        
        private bool   _editable;
        private bool   _toggleState;
        private bool   _focus;
        private Note   _note;

        // =======================================================================
        protected void OnEnable()
        {
            _note = (Note)target;
            
            _viewer = new MarkdownViewer( Preferences.DarkSkin ? SkinDark : SkinLight, "Assets/", _getText());
            EditorApplication.update += UpdateRequests;
        }

        protected void OnDisable()
        {
            EditorApplication.update -= UpdateRequests;
            _viewer = null;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        private string _getText()
        {
            if (string.IsNullOrEmpty(_note._text))
                return "empty";
            
            if (_note._showHeader)
                return $"## {target.name}\n\n{_note._text}";
            
            return _note._text;
        }

        public override void OnInspectorGUI()
        {
            if (string.IsNullOrEmpty(_note._text))
                _editable = true;
            
            // capture event before use
            
            if (_editable == false)
            {
                var toggle = Event.current.type == EventType.KeyDown && ((Event.current.keyCode == KeyCode.Return) || (Event.current.keyCode == KeyCode.Space));
                
                if (Event.current.isMouse && Event.current.button == 0 || toggle)
                {
                    _editable = true;
                    _focus = true;
                }
                
                _viewer.Draw();
            }
            
            if (_editable)
            {
                GUI.SetNextControlName("Header");
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Note._showHeader)));
                GUI.SetNextControlName("View");
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Note._view)));
                
                var toggle = Event.current.type == EventType.KeyDown && Event.current.shift && Event.current.keyCode == KeyCode.Return;
                
                 GUI.SetNextControlName(k_TextControlId);
                _note._text = GUILayout.TextArea(_note._text);
                
                if (_focus)
                {
                    GUI.FocusControl(k_TextControlId);
                    _focus = false;
                }
                
                if (toggle || (GUI.GetNameOfFocusedControl() != k_TextControlId && GUI.GetNameOfFocusedControl() != "Header" && GUI.GetNameOfFocusedControl() != "View"))
                {
                    _editable = false;
                    _viewer   = new MarkdownViewer(Preferences.DarkSkin ? SkinDark : SkinLight, "Assets/", _getText());
                }
                
                serializedObject.ApplyModifiedProperties();
            }
            

            // -----------------------------------------------------------------------
            int _getNumberOfLines(string text)
            {
                var content = Regex.Replace(text, @"\r\n|\n\r|\r|\n", Environment.NewLine);
                var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                return lines.Length;
            }

            float _getTextAreaHeight(string text)
            {
                var height = (EditorGUIUtility.singleLineHeight - 3.0f) * _getNumberOfLines(text) + 3.0f;
                return height;
            }
        }

        void UpdateRequests()
        {
            if( _viewer.Update() )
            {
                Repaint();
            }
        }
    }
}
