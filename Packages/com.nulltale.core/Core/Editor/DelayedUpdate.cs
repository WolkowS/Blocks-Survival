using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using Task = System.Threading.Tasks.Task;

namespace CoreLib
{
    public static class DelayedUpdate
    {
        private static Dictionary<string, UpdateData> s_Coroutines = new Dictionary<string, UpdateData>();

        // =======================================================================
        private class UpdateData
        {
            public double          StartTime;
            public double          Delay;
            public bool            Running;
        }

        // =======================================================================
        public static void Plan(Action action, double delay = 2d)
        {
            Plan(action.Method.Name, delay, action);
        }

        public static void Plan(string key, double delay, Action action)
        {

            if (s_Coroutines.ContainsKey(key) == false)
                s_Coroutines.Add(key, new UpdateData());

            var updateData = s_Coroutines[key];
            updateData.StartTime = EditorApplication.timeSinceStartup;
            updateData.Delay     = delay;

            if (updateData.Running)
                return;

            updateData.Running = true;
            _delayedUpdate();

            // ===================================
            async void _delayedUpdate()
            {
                while (EditorApplication.timeSinceStartup - updateData.StartTime < updateData.Delay)
                    await Task.Yield();

                action?.Invoke();
                updateData.Running = false;
            }
        }
    }
}