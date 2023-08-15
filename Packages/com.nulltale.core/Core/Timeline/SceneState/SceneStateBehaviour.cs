using CoreLib.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class SceneStateBehaviour : PlayableBehaviour
    {
        private SceneState m_SceneState;
        private bool       m_Broken;

        public bool Broken
        {
            get => m_Broken;
            set
            {
                if (m_Broken == value)
                    return;

                m_Broken = value;

                if (m_Broken)
                    Debug.LogWarning($"Can't get scene state");
            }
        }

        // =======================================================================
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            m_Broken = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Broken)
                return;

            // get SceneState from id and current current SceneData
            if (m_SceneState.IsNull() && playerData.IsNull() == false)
            {
                var director = playable.GetGraph().GetResolver() as PlayableDirector;
                if (director.IsNull())
                {
                    Broken = true;
                    return;
                }

                if (SceneManager.Instance.m_SceneData.TryGetValue(director.gameObject.scene.handle, out var sceneData) == false)
                {
                    Broken = true;
                    return;
                }

                if (sceneData.m_States.TryGetValue((IdAsset)playerData, out m_SceneState) == false)
                {
                    Broken = true;
                    return;
                }

                m_SceneState.Open();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_SceneState != null)
            {
                m_SceneState.Close();
                m_SceneState = null;
            }
        }
    }
}