using CoreLib.States;
using UnityEngine;

namespace Fungus
{
    [EventHandlerInfo("",
                      "On State",
                      "The block will execute when the global state opened.")]
    [AddComponentMenu("")]
    public class OnGlobalState : EventHandler
    {
        [SerializeField] protected Gs _state;

        private void OnEnable()
        {
            _state.OnOpen += _execute;
        }

        private void OnDisable()
        {
            _state.OnOpen -= _execute;
        }

        private void _execute() => ExecuteBlock();
    }
}