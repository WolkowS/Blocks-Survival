using UnityEngine;

namespace CoreLib
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public sealed class SingletonSetter : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<ISingleton>().SetupSingleton();
        }
    }
}