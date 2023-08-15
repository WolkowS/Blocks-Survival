using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    [CreateAssetMenu(fileName = nameof(SceneManager), menuName = Core.k_CoreModuleMenu + nameof(SceneManager))]
    public class SceneManager : Core.Module<SceneManager>
    {
        public Dictionary<ScenePreset, List<SceneArgs>> m_SceneArgs = new Dictionary<ScenePreset, List<SceneArgs>>();
        public Dictionary<int, SceneData>               m_SceneData = new Dictionary<int, SceneData>();

        public Optional<float> m_SceneArgsLifetime;

        public SoCollection<ScenePreset>      m_Scenes;
        public SoCollection<TransitionPreset> m_Transitions;

        // =======================================================================
        public override void Init()
        {
            m_SceneArgs = new Dictionary<ScenePreset, List<SceneArgs>>();
            m_SceneData = new Dictionary<int, SceneData>();
        }

    }
}