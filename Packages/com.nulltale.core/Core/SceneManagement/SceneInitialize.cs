using CoreLib.Values;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder + 1)]
    public class SceneInitialize : MonoBehaviour
    {
        public Optional<Vers<int>> _priority;
        
        private void Awake()
        {
            SceneInitializer.AddInitializer(GetComponent<IInitializable>(), _priority.Enabled ? _priority.Value.Value : 0);
        }
    }
}