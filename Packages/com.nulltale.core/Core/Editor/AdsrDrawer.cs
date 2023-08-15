using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(Adsr), true)]
    internal sealed class AdsrDrawer : PropertyDrawer
    { 
        const int propCount = 4;
        bool fold;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return !fold ? h : propCount * h - EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var h = EditorGUIUtility.singleLineHeight;
            var fullWidth = position.width;

            EditorGUI.BeginProperty(position, label, property);

            var startX = position.x;

            // Draw label foldout
            var foldRect = position;
            foldRect.height = h;
            fold = EditorGUI.Foldout(foldRect, fold, label, true);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(" "));

            var attackProp = property.FindPropertyRelative("attack");
            var decayProp = property.FindPropertyRelative("decay");
            var sustainProp = property.FindPropertyRelative("sustain");
            var releaseProp = property.FindPropertyRelative("release");

            var attackEaseProp = property.FindPropertyRelative("attackEase");
            var decayEaseProp = property.FindPropertyRelative("decayEase");
            var releaseEaseProp = property.FindPropertyRelative("releaseEase");

            var interruptProp = property.FindPropertyRelative("interrupt");
            var outputProp = property.FindPropertyRelative("output");
            
            var adsr = new Adsr
            (
                attackProp.floatValue,
                decayProp.floatValue,
                sustainProp.floatValue,
                releaseProp.floatValue,

                attackEaseProp.floatValue,
                decayEaseProp.floatValue,
                releaseEaseProp.floatValue,

                interruptProp.boolValue
            );

            var curveRect = position;

            var spacing = 0.7f;
            /*if (fold == false)
            {*/
                curveRect.height = h;
            /*}
            else
            {
                curveRect.x = startX;
                curveRect.y = position.y + h;

                curveRect.width = fullWidth;
                curveRect.height = h * spacing;
            }*/

            var curveStart = curveRect.position - new Vector2(0, -curveRect.height);
            var lastP = curveStart;
            var viewWidth = (int)curveRect.width;

            var os = 0.75f;
            var adrScale = (attackProp.floatValue + decayProp.floatValue + releaseProp.floatValue) / (viewWidth * os);
            if (adrScale == 0f)
                adrScale = 1f;

            var attackWidth  = attackProp.floatValue / adrScale;
            var decayWidth   = decayProp.floatValue / adrScale;
            var releaseWidth = releaseProp.floatValue / adrScale;

            var focused = GUI.GetNameOfFocusedControl();
            
            var idleColor = new Color(0.18f, 0.18f, 0.18f);
            var editColor = new Color(0.25f, 0.25f, 0.25f);
            
            var miniRect = curveRect;
            miniRect.x = (int)miniRect.x;
            miniRect.width = Mathf.CeilToInt(attackWidth);
            EditorGUI.DrawRect(miniRect, focused == "attack" ? editColor : idleColor);

            miniRect.x += miniRect.width + spacing;
            miniRect.width = Mathf.CeilToInt(decayWidth) - spacing;
            EditorGUI.DrawRect(miniRect, focused == "decay" ? editColor : idleColor);

            miniRect.x += miniRect.width + spacing;
            miniRect.width = Mathf.CeilToInt(curveRect.width - (attackWidth + decayWidth + releaseWidth)) - spacing;
            EditorGUI.DrawRect(miniRect, focused == "sustain" ? editColor : idleColor);

            miniRect.x = Mathf.CeilToInt(curveRect.x + viewWidth - releaseWidth) + spacing;
            miniRect.width = (int)releaseWidth - spacing;
            EditorGUI.DrawRect(miniRect, focused == "release" ? editColor : idleColor);

            lastP = curveStart + new Vector2(0f, -adsr.EvaluateIn(0f) * curveRect.height);
            var graphScale = adrScale;
            Handles.color = new Color(0.12f, 0.85f, 0.33f);
            for (var i = 0; i < viewWidth; i++)
            {
                var v = i < viewWidth - releaseWidth ?
                    adsr.EvaluateIn(i * graphScale) :
                    adsr.EvaluateOut((i - (viewWidth - releaseWidth)) * graphScale);

                var p = curveStart + new Vector2(i, -v * curveRect.height);
                Handles.DrawLine(lastP, p);
                lastP = p;
            }

            if (fold)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel ++;

                position.x = startX;
                //position.y += curveRect.height + h + 5;
                position.y += h + EditorGUIUtility.standardVerticalSpacing;
                position.width = fullWidth;

                EditorGUIUtility.labelWidth = 52;

                var margin    = spacing;
                var propWidth = position.width / (4);
                var valueRect = new Rect(position.x, position.y, propWidth - 10, h);
                var easeRect  = new Rect(position.x + valueRect.width, position.y, valueRect.width, h);

                GUI.SetNextControlName("attack");
                EditorGUI.PropertyField(valueRect, attackProp); valueRect.x += propWidth + margin;
                GUI.SetNextControlName("decay");
                EditorGUI.PropertyField(valueRect, decayProp); valueRect.x += propWidth + margin;
                GUI.SetNextControlName("sustain");
                EditorGUI.PropertyField(valueRect, sustainProp); valueRect.x += propWidth + margin;
                GUI.SetNextControlName("release");
                EditorGUI.PropertyField(valueRect, releaseProp); valueRect.x = startX;
                
                valueRect.y += h + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(valueRect, attackEaseProp, new GUIContent("Ease")); valueRect.x += propWidth + margin;
                EditorGUI.PropertyField(valueRect, decayEaseProp, new GUIContent("Ease")); valueRect.x += propWidth + margin;
                EditorGUI.PropertyField(valueRect, interruptProp); valueRect.x += propWidth + margin;
                EditorGUI.PropertyField(valueRect, releaseEaseProp, new GUIContent("Ease")); valueRect.x += propWidth + margin;
                
                valueRect.y += h + EditorGUIUtility.standardVerticalSpacing;
                valueRect.x = startX;
                valueRect.width = position.width;
                EditorGUI.PropertyField(valueRect, outputProp);


                if (attackProp.floatValue < 0)
                    attackProp.floatValue = 0;

                if (decayProp.floatValue < 0)
                    decayProp.floatValue = 0;

                if (releaseProp.floatValue < 0)
                    releaseProp.floatValue = 0;

                sustainProp.floatValue = Mathf.Clamp01(sustainProp.floatValue);

                EditorGUI.indentLevel --;

                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }
    }
}