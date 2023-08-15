using UnityEngine;

namespace CoreLib
{
    public class RegistryAsset : IdAsset
    {
        public GameObject _prefab;

        protected virtual void OnValidate()
        {
            if (_prefab != null)
                if (_prefab.TryGetComponent<IRegistryEntity>(out var asset))
                {
                    asset.ValidateAsset(this);
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty((Object)asset);
#endif
                }
        }
    }
    
    public interface IRegistryEntity
    {
        internal void ValidateAsset(RegistryAsset asset);
    }
}