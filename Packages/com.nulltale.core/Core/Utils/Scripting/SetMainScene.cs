using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreLib.Scripting
{
    public class SetMainScene : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.SetActiveScene(gameObject.scene);
        }
    }
}