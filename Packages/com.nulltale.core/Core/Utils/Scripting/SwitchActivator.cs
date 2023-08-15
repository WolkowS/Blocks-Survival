using UnityEngine;

namespace CoreLib.Scripting
{
    public class SwitchActivator : MonoBehaviour
    {
        public SwitchBase _switch;

        // =======================================================================
        private void OnEnable()
        {
            _switch.Up();
        }

        private void OnDisable()
        {
            _switch.Down();
        }
    }
}