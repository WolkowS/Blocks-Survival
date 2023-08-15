using UnityEngine;

namespace CoreLib.Scripting
{
    [ExecuteAlways]
    public class LinePoint : MonoBehaviour
    {
        private LineRenderer _line;

        // =======================================================================
        private void Awake()
        {
            _line = GetComponentInParent<LineRenderer>();
        }

        private void Update()
        {
            _line.SetPosition(transform.GetSiblingIndex(), transform.position);
        }
    }
}