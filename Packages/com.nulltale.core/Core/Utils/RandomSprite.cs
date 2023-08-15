using CoreLib;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class RandomSprite : MonoBehaviour
    {
        public Sprite[] _options;

        // =======================================================================
        private void Awake() => Invoke();

        public void Invoke()
        {
            if (TryGetComponent(out SpriteRenderer sr))
                sr.sprite = _options.Random();

            if (TryGetComponent(out Image im))
                im.sprite = _options.Random();
        }
    }
}