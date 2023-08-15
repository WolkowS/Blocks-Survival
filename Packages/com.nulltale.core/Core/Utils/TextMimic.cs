using TMPro;
using UnityEngine;

namespace CoreLib
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextMimic : MonoBehaviour
    {
        public  TMP_Text _target;
        private TMP_Text _text;

        // =======================================================================
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.text = _target.text;
        }
    }
}