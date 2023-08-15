using UnityEngine;
using UnityEditor;


namespace CoreLib.Timeline
{
    [CustomPropertyDrawer(typeof(TransformBehaviour))]
    public class TransformTweenDrawer : PropertyDrawer
    {
        private GUIContent m_TweenTypeContent = new GUIContent("Interpolation", "Linear - the transform moves the same amount each frame (assuming static start and end locations).\n"
                                                                             + "Deceleration - the transform moves slower the closer to the end location it is.\n"
                                                                             + "Harmonic - the transform moves faster in the middle of its tween.\n"
                                                                             + "Custom - uses the customStartingSpeed and customEndingSpeed to create a curve for the desired tween.");

        private GUIContent m_CustomCurveContent = new GUIContent("Custom Curve", "This should be a normalised curve (between 0,0 and 1,1) that represents how the tweening object accelerates at different points along the clip.");

        // =======================================================================
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldCount = property.FindPropertyRelative("tween").enumValueIndex == (int)TransformBehaviour.TweenType.Custom ? 5 : 4;
            return fieldCount * (EditorGUIUtility.singleLineHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tweenPositionProp = property.FindPropertyRelative("position");
            var tweenRotationProp = property.FindPropertyRelative("rotation");
            var tweenScaleProp    = property.FindPropertyRelative("scale");
            var tweenTypeProp     = property.FindPropertyRelative("tween");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, tweenPositionProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, tweenRotationProp);
            
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, tweenScaleProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, tweenTypeProp, m_TweenTypeContent);

            if (tweenTypeProp.enumValueIndex == (int)TransformBehaviour.TweenType.Custom)
            {
                var customCurveProp = property.FindPropertyRelative("customCurve");

                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, customCurveProp, m_CustomCurveContent);
            }
        }
    }
}