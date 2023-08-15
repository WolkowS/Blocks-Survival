using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.AnimatorBehaviour
{
    public class OnEnter : StateMachineBehaviour
    {
        public  UnityEvent _onInvoke;
        
        // =======================================================================
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onInvoke.Invoke();
        }
        
    }
}