using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.Scripting
{
    public class InvokeButton : MonoBehaviour
    {
        public void Invoke()
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}