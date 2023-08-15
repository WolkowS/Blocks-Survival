using UnityEngine;

namespace CoreLib.AnimatorBehaviour
{
    public class SyncTime : StateMachineBehaviour
    {
        private bool _lock;
        
        // =======================================================================
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_lock)
            {
                _lock = false;
                return;
            }
            
            animator.Play(stateInfo.fullPathHash, layerIndex, (Time.time % stateInfo.length) / stateInfo.length);
            _lock = true;
        }
    }
}