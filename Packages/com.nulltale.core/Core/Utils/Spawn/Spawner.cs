using System;
using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Spawn
{
    public class Spawner : MonoBehaviour, IInvokable
    {
        [SerializeField]
        private Vers<GameObject> m_Prefab;
        [SerializeField]
        private Parent           m_Parent;
        [SerializeField]
        [ShowIf(nameof(m_Parent), Parent.Target)]
        private Vers<GameObject> m_SetParent;
        [SerializeField]
        private bool             m_SetPosition = true;
        [SerializeField]
        private bool             m_SetRotation;
        [SerializeField]
        private bool             m_SetActive;
        [SerializeField] [Label("Root Child")]
        private bool             m_RootMode;
        [SerializeField] [ShowIf(nameof(m_RootMode))]
        private Optional<Vers<int>> m_ChildIndex;
        [SerializeField] [ShowIf(nameof(m_RootMode))]
        private Optional<Vers<string>> m_ChildName;
        [SerializeField]
        private bool             m_AsSubPrefab;
        
        public Optional<Deployer> m_Deployer;

        // =======================================================================
        [Serializable]
        public enum Parent
        {
            World,
            Self,
            Parent,
            Target,
        }

        // =======================================================================
        private void Awake()
        {
            if (m_AsSubPrefab)
            {
                var inst = Instantiate(m_Prefab.Value, transform);
                m_Prefab.Value = inst;
                inst.gameObject.SetActive(false);
            }
        }

        [Button]
        public void Invoke()
        {
            Invoke(m_Prefab.Value);
        }
        
        public void Invoke(Vector3 pos)
        {
            var prefab = m_Prefab.Value;
            if (m_RootMode)
            {
                if (m_ChildName.Enabled == false && m_ChildIndex.Enabled == false)
                    prefab = prefab.GetChildren().Random().gameObject;
                
                if (m_ChildIndex.Enabled)
                    prefab = prefab.transform.GetChild(m_ChildIndex.Value.Value).gameObject;
                
                if (m_ChildName.Enabled)
                    prefab = prefab.transform.GetChildren().First(n => n.name == m_ChildName.Value.Value).gameObject;
            }

            var instance = m_Parent switch
            {
                Parent.World  => Instantiate(prefab, null),
                Parent.Self   => Instantiate(prefab, transform),
                Parent.Parent => Instantiate(prefab, transform.parent),
                Parent.Target => Instantiate(prefab, m_SetParent.Value.transform),
                _             => throw new ArgumentOutOfRangeException(),
            };
            
            var rot = transform.rotation;
            instance.transform.position = pos;
            
            if (m_SetRotation)
                instance.transform.rotation = rot;
            if (m_SetActive)
                instance.SetActive(true);
        }

        public void Invoke(GameObject prefab)
        {
            if (prefab == null)
                return;
            
            if (m_RootMode)
            {
                if (m_ChildName.Enabled == false && m_ChildIndex.Enabled == false)
                    prefab = prefab.GetChildren().Random().gameObject;
                
                if (m_ChildIndex.Enabled)
                    prefab = prefab.transform.GetChild(m_ChildIndex.Value.Value).gameObject;
                
                if (m_ChildName.Enabled)
                    prefab = prefab.transform.GetChildren().First(n => n.name == m_ChildName.Value.Value).gameObject;
            }

            var instance = m_Parent switch
            {
                Parent.World  => Instantiate(prefab, null),
                Parent.Self   => Instantiate(prefab, transform),
                Parent.Parent => Instantiate(prefab, transform.parent),
                Parent.Target => Instantiate(prefab, m_SetParent.Value.transform),
                _             => throw new ArgumentOutOfRangeException(),
            };
            
            var pos = transform.position;
            var rot = transform.rotation;
            
            if (m_Deployer.Enabled)
                m_Deployer.Value.Locate(out pos, out rot);

            if (m_SetPosition)
                instance.transform.position = pos;
            if (m_SetRotation)
                instance.transform.rotation = rot;
            if (m_SetActive)
                instance.SetActive(true);
        }
    }
}
