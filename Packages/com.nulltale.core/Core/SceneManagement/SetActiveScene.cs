using System.Collections;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(-10000)]
    public class SetActiveScene : MonoBehaviour
    {
        public Event _event;
        
        // =======================================================================
        public enum Event
        {
            Awake,
            Start,
            Late,
            Manual
        }
        
        // =======================================================================
        private void Awake()
        {
            if (_event == Event.Awake)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(gameObject.scene);
        }
        
        private IEnumerator Start()
        {
            if (_event == Event.Start)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(gameObject.scene);
            
            yield return Core.k_WaitForEndOfFrame;
            
            if (_event == Event.Late)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(gameObject.scene);
        }
        
        public void Invoke()
        {
            if (_event == Event.Manual)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(gameObject.scene);
        }
    }
}