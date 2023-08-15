using UnityEngine;

namespace CoreLib.Values
{
    public class ActivationImpact : MonoBehaviour
    {
        public Vers<GameObject> _value;

        // =======================================================================
        private void OnEnable()
        {
            _value.Value.SetActive(true);
        }

        private void OnDisable()
        {
            _value.Value.SetActive(false);
        }
    }
}