using System.Collections;
using System.Collections.Generic;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.AnimatorBehaviour
{
    public class RandomTime : StateMachineBehaviour
    {
        private bool    _lock;
        
        // =======================================================================
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_lock)
            {
                _lock = false;
                return;
            }
            
            animator.Play(stateInfo.fullPathHash, layerIndex, Random.value);
            _lock = true;
        }
    }
}
