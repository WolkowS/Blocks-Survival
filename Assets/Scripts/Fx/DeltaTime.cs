using UnityEngine;

namespace Game
{
    public class DeltaTime : MonoBehaviour
    {
        public bool _unscaled;
        
        public float Delta => _unscaled ? Time.deltaTime : Time.unscaledDeltaTime;
    }
}