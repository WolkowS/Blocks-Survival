using System;

namespace CoreLib
{
    public class OnGUICallback : CallbackBase
    {
        public Action Action;
	
        // =======================================================================
        private void OnGUI()
        {
            Action.Invoke();
        }
    }
}