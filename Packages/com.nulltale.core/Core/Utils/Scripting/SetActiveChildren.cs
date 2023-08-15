using System.Linq;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetActiveChildren : MonoBehaviour
    {
        public void SetCount(int count)
        {
            var array = gameObject.GetChildren().ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var child = array[index];
                child.gameObject.SetActive(index < count); 
            }
        }
        public void SetIndex(int index)
        {
            var array = gameObject.GetChildren().ToArray();
            for (var n = 0; n < array.Length; n++)
            {
                var child = array[n];
                child.gameObject.SetActive(n <= index); 
            }
        }
        
        public void Next()
        {
            var array = gameObject.GetChildren().ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var child = array[index];
                if (child.gameObject.activeSelf == false)
                {
                    child.gameObject.SetActive(true);
                    return;
                }
            }
        }
    }
}