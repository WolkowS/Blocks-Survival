using System.Linq;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetActiveChild : MonoBehaviour
    {
        public bool m_Invert;
        
        // =======================================================================
        public void Invoke(int index)
        {
            var array = transform.GetChildren().ToArray();
            for (var n = 0; n < array.Length; n++)
            {
                var child = array[n];
                if (n == index)
                    _setActive(child, true);
                else
                    _setActive(child, false);
            }
        }
        
        public void Invoke(bool val)
        {
            foreach (var child in transform.GetChildren())
                _setActive(child, val);
        }

        private void _setActive(Transform child, bool val)
        {
            var setActive = m_Invert ? !val : val;
            child.gameObject.SetActive(setActive);
        }

        public void Random()
        {
            Invoke(false);
            
            var children = transform.GetChildren().RandomOrDefault();
            if (children != null)
                _setActive(children, true);
        }
        
        public void Random(int count)
        {
            Invoke(false);
            
            var children = transform.GetChildren().ToList();
            foreach (var child in children.Randomize().Take(count))
                _setActive(child, true);
        }
    }
}