using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.ExposedValues
{
    [CustomPropertyDrawer(typeof(ExposedValue<>), true)]
    public class ExposedValueLinkDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 0 && _isLinkOwner() == false)
            {
                EditorGUIUtility.PingObject((Object)property.objectReferenceValue);
            }

            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 0 && Event.current.clickCount == 2)
            {
                _link();
            }

            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition) && Event.current.button == 1)
            {
                var context = new GenericMenu();

                if (property.objectReferenceValue == null || _isLinkOwner() == false)
                    context.AddItem(new GUIContent("Create"), false, () =>
                    {
                        if (property.objectReferenceValue)
                            _clear();

                        var valueLink = (ExposedValue)((MonoBehaviour)property.serializedObject.targetObject).gameObject.AddComponent(property.GetFieldType());
                        valueLink.m_Owner             = property.serializedObject.targetObject;
                        valueLink.m_ID                = property.name;
                        //valueLink.hideFlags           = HideFlags.HideInInspector;
                        property.objectReferenceValue = valueLink;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    });
                if (property.objectReferenceValue)
                    context.AddItem(new GUIContent("Clear"), false, () =>
                    {
                        if (property.objectReferenceValue)
                        {
                            _clear();
                            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                    });
                //context.AddItem(new GUIContent("Link"), false, _link);

                // show menu
                context.ShowAsContext();
            }

            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            using var so = new SerializedObject(property.objectReferenceValue);

            EditorGUI.BeginChangeCheck();

            var prop  = so.FindProperty("m_Value");
            var value = (ExposedValue)property.objectReferenceValue;
            EditorGUI.PropertyField(position, prop, _isLinkOwner() ? label : new GUIContent($"{property.displayName} ({value.Path})"));

            var changed = EditorGUI.EndChangeCheck();
            if (changed)
                so.ApplyModifiedProperties();

            // ===================================
            bool _isLinkOwner()
            {
                var val = (ExposedValue)property.objectReferenceValue;
                if (val == null)
                    return false;

                return val.m_Owner == property.serializedObject.targetObject && val.m_ID == property.name;
            }

            void _clear()
            {
                var val = (ExposedValue)property.objectReferenceValue;
                if (_isLinkOwner())
                    Object.DestroyImmediate(property.objectReferenceValue);

                property.objectReferenceValue = null;
            }

            void _link()
            {
                var valueLink = (ExposedValue)property.objectReferenceValue;
                var values    = Object.FindObjectsOfType<ExposedValue>(false).Except(valueLink).ToList();
                ObjectPickerWindow.Show<ExposedValue>(n =>
                {
                    if (property.objectReferenceValue)
                        _clear();

                    property.objectReferenceValue = (ExposedValue)n;
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }, (ExposedValue)property.objectReferenceValue, values, 0, n => new GUIContent($"{n.m_Owner.name}.{n.Path}"), onSelected: n => { EditorGUIUtility.PingObject((Object)n); });
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            using var so = new SerializedObject(property.objectReferenceValue);
            return EditorGUI.GetPropertyHeight(so.FindProperty("m_Value"), true);
        }
    }
}