using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Windows;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(AutoSetAttribute))]
    public class AutoSetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var comp = _getComponent(property);
            if (property.objectReferenceValue != comp)
                property.objectReferenceValue = comp;

            if (((AutoSetAttribute)attribute).HideInInspector == false && comp == null)
            {
                GUI.color   = Color.red;
                GUI.Box(position, GUIContent.none);
                GUI.color   = Color.white;
                
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, new GUIContent(label.text, "AutoSet component is missing"), true);
                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var comp = _getComponent(property);
            if (((AutoSetAttribute)attribute).HideInInspector == false && comp == null)
                return EditorGUI.GetPropertyHeight(property, label, true);

            return 0f;
        }

        private Object _getComponent(SerializedProperty prop)
        {
            var attr = ((AutoSetAttribute)attribute);
            
            var go   = ((MonoBehaviour)prop.serializedObject.targetObject).gameObject;
            var type = attr.OfType == null ? prop.GetFieldType() : attr.OfType;

            Component comp = null;

            if (attr.Path.IsNullOrEmpty() == false)
            {
                go = go.transform.Find(attr.Path)?.gameObject;
                if (go == null)
                    return null;
            }
            
            if (attr.InSelf)
            {
                comp = go.GetComponent(type);

                if (comp != null)
                    return comp;
            }
            
            if (attr.InParent)
            {
                comp = go.transform.parent != null ? go.transform.parent.GetComponentInParent(type, true) : null;

                if (comp != null)
                    return comp;
            }
            
            if (attr.InChildren)
            {
                comp = go.GetComponentInChildren(type, true);
                if (comp != null)
                    return comp;
            }

            return null;
        }
    }

    /*public class AutoSetProcessor : AssetPostprocessor
    {
        private void OnPostprocessPrefab(GameObject g)
        {
        }
    }*/
}