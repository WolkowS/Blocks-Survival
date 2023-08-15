using System;
using CoreLib.Render.RenderFeature;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(Blit.TextureSource))]
    public class TextureSourceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type  = property.FindPropertyRelative(nameof(Blit.TextureSource._type));
            var asset = property.FindPropertyRelative(nameof(Blit.TextureSource._asset));
            var id    = property.FindPropertyRelative(nameof(Blit.TextureSource._id));
            var index = 0;
            
            EditorGUI.PropertyField(position.GuiField(index ++), type, label);
			
            switch ((Blit.Target)type.intValue)
            {
                case Blit.Target.CameraColor:
                    break;
                case Blit.Target.TextureId:
                {
                    EditorGUI.PropertyField(position.GuiField(index ++), id);
                } break;
                case Blit.Target.TextureAsset:
                {
                    EditorGUI.PropertyField(position.GuiField(index ++), asset);
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var type = property.FindPropertyRelative(nameof(Blit.TextureSource._type));
            var lines = 1 + (Blit.Target)type.intValue switch
            {
                Blit.Target.CameraColor  => 0,
                Blit.Target.TextureId    => 1,
                Blit.Target.TextureAsset => 1,
                _                        => throw new ArgumentOutOfRangeException()
            };
			
            return lines * EditorGUIUtility.singleLineHeight;
        }
    }
}
