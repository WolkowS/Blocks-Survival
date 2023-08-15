using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(GlobalValues), menuName = Core.k_CoreModuleMenu + nameof(GlobalValues))]
    public class GlobalValues : Core.Module<GlobalValues>
    {
        [HideInInspector]
        public List<GlobalValue> m_Values;
        
        internal PlayableUpdate m_PlayableUpdate;
        [ReadOnly]
        public   bool           m_AutoCollect = true;

        // =======================================================================
        internal class PlayableUpdate : MonoBehaviour
        {
            public List<IPlayableUpdate> m_Updates = new List<IPlayableUpdate>();

            // =======================================================================
            private void Awake()
            {
                hideFlags = HideFlags.HideAndDontSave;
            }

            private void LateUpdate()
            {
                if (m_Updates.Count == 0)
                    return;

                foreach (var update in m_Updates)
                    update.Update();

                m_Updates.Clear();
            }
        }

        // =======================================================================
        public override void Init()
        {
            
#if UNITY_EDITOR
            m_Values = Extensions.FindAssets<GlobalValue>().ToList();
            
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            
            m_PlayableUpdate = Core.Instance.gameObject.AddComponent<PlayableUpdate>();

            foreach (var value in m_Values)
                value.Init();
        }
    }
}