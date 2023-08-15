using System.Linq;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class DisableChildren : MonoBehaviour
    {
        public bool _onEnable;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            foreach (var trans in transform.parent.GetChildren().Where(n => n != transform))
                trans.gameObject.SetActive(false);
        }
    }
}