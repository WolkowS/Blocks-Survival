using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using CoreLib.Module;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.States
{
    [CreateAssetMenu(fileName = nameof(GlobalStates), menuName = Core.k_CoreModuleMenu + nameof(GlobalStates))]
    public class GlobalStates : Core.Module<GlobalStates>
    {
        [SerializeField]
        private bool m_ShowStates;
        [HideInInspector]
        public List<GlobalStateBase> m_Runtime;
        [ReadOnly]
        public bool m_AutoCollect = true;

        public bool ShowStates
        {
            get => m_ShowStates;
            set
            {
                if (m_ShowStates == value)
                    return;

                m_ShowStates = value;
                m_StateLogger.enabled = m_ShowStates;
            }
        }

        private StateLogger m_StateLogger;

        // =======================================================================
        public class StateLogger : MonoBehaviour
        {
            private void Update()
            {
                if (DebugTools.Instance.IsNull())
                    return;

                foreach (var state in Instance.m_Runtime)
                    DebugTools.ScreenLog(state.name, state.IsOpen);
            }
        }

        // =======================================================================
        public override void Init()
        {
#if UNITY_EDITOR
            if (m_AutoCollect)
            {
                m_Runtime = Extensions.FindAssets<GlobalStateBase>().ToList();
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            
            foreach (var state in m_Runtime)
                state.Init();

            Terminal.Shell?.AddCommand("GlobalStates", args =>
            {
                // toggle if empty
                if (args.IsEmpty())
                {
                    ShowStates = !ShowStates;
                    return;
                }

                ShowStates = args[0].Bool;
            },0, 1, "Show GlobalStates");

            _validateStateLogger();
        }

        private void OnValidate()
        {
            _validateStateLogger();
        }

        public static WaitStateYieldInstruction WaitTaskGroup(GlobalStateBase state, bool waitForClosing)
        {
            return new WaitStateYieldInstruction(state, waitForClosing);
        }

        [ContextMenu("Init States")]
        public void InitStates()
        {
            foreach (var state in m_Runtime)
                state.Init();
        }

        // =======================================================================
        private void _validateStateLogger()
        {
            if (Application.isPlaying == false)
                return;

            if (Core.Instance == null)
                return;

            if (Core.Instance.gameObject.TryGetComponent(out m_StateLogger) == false)
            {
                m_StateLogger           = Core.Instance.gameObject.AddComponent<StateLogger>();
                m_StateLogger.hideFlags = HideFlags.NotEditable;
                m_StateLogger.enabled   = m_ShowStates;
            }
        }
    }
}