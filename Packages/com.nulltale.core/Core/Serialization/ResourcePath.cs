using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreLib.Serializer
{
    [Serializable]
    public class ResourcePath : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string m_Path;

        public string Path => m_Path;

        // =======================================================================
        public void OnValidate()
        {
            _updateResourcePath();
        }

        public void OnBeforeSerialize()
        {
            _updateResourcePath();
        }

        public void OnAfterDeserialize()
        {
        }

        // =======================================================================
        private void _updateResourcePath()
        {
#if UNITY_EDITOR
            if (gameObject == null)
                return;

            var path = Serialization.AssetPathToResourcePath(
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject));
            if (string.IsNullOrEmpty(path) == false && m_Path != path)
            {
                m_Path = path;
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
#endif
        }
    }
}