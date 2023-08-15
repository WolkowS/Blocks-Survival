using CoreLib;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    [DefaultExecutionOrder(1000)]
    public class TilemapFix : MonoBehaviour
    {
        private void Start()
        {
            var tilemap = gameObject;

            tilemap.SetActive(false);
            Core.Instance.Delayed(() => tilemap.SetActive(true), 1);
        }
    }
}