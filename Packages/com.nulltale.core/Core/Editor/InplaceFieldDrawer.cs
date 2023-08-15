using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(InplaceFieldAttribute))]
    public class InplaceFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pos = position.WithHeight(0f);

            var attr = ((InplaceFieldAttribute)attribute);
            // if (attr.PropertyPath == null || attr.PropertyPath.Length == 0)
            // {
            //     foreach (var prop in property.GetEnumerator().ToEnumerable().OfType<SerializedProperty>())
            //     {
            //         pos.y      += pos.height;
            //         pos.height =  EditorGUI.GetPropertyHeight(prop, true);
            //         EditorGUI.PropertyField(pos, prop, true);
            //     }
            // }
            
            foreach (var propPath in attr.PropertyPath)
            {
                var prop   = property.FindPropertyRelative(propPath);
                pos.y     += pos.height;
                pos.height =  EditorGUI.GetPropertyHeight(prop, true);
                EditorGUI.PropertyField(pos, prop, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ((InplaceFieldAttribute)attribute).PropertyPath.Sum(n => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(n), true));
        }

        /*private string _propertyPaths(SerializedProperty property)
        {
            if (((InplaceFieldAttribute)attribute).PropertyPath == null)
                return property.GetEnumerator()

        }*/
    }
}