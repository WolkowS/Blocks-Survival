using System;
using System.Collections;
using UnityEngine;

namespace CoreLib.Scripting
{
    [DefaultExecutionOrder(-1000)]
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField]
        private Mode _mode;

        // =======================================================================
        [Serializable]
        public enum Mode
        {
            None,
            Unparent,
            MoveParent
        }
        
        // =======================================================================
        private void Awake()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                switch (_mode)
                {
                    case Mode.Unparent:
                    { // set this object to the root
                        gameObject.transform.SetParent(null, true);
                        StartCoroutine(_waitFrameAndDo());
                    } break;
                    case Mode.MoveParent:
                    { // don't destroy on load root
                        GameObject.DontDestroyOnLoad(gameObject.transform.root);
                    } break;
                    case Mode.None:
                        break;
                    default:
                        break;
                }
            }
        }

        // =======================================================================
        private IEnumerator _waitFrameAndDo()
        {
            yield return null;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
}
