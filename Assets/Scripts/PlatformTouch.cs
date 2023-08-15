using UnityEngine;

namespace Game
{
    public class PlatformTouch : MonoBehaviour
    {
        public void Invoke(Collider2D col)
        {
            var plat = col.GetComponentInParent<Platform>();
            if (plat == null)
                return;
            
            plat.Hit();
        }
    }
}