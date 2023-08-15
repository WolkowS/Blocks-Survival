using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Menu text", 
                 "Set choose text")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class MenuText : Command
    {
        [Tooltip("Name of a label in this block to jump to")]
        [SerializeField] protected StringData _text = new StringData("");

        // =======================================================================
        public override void OnEnter()
        {
            if (_text.Value == "")
            {
                Continue();
                return;
            }

            
            var menuDialog = MenuDialog.GetMenuDialog();
            if (menuDialog != null)
            {
                menuDialog.gameObject.SetActive(true);
                menuDialog.SetText(_text);
            }
            
            Continue();
        }

        public override string GetSummary()
        {
            if (_text.Value == "")
                return "Error: Text not set";

            return _text.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _text.stringRef == variable || base.HasReference(variable);
        }
    }
}