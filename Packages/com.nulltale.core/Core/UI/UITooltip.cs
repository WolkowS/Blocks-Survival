using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class UITooltip : MonoBehaviour
    {
        public static UITooltip Instance;
        
        public TMP_Text _text;
        public Mode _mode;

        public UnityEvent<string> _onShow;
        public UnityEvent         _onHide;

        // =======================================================================
        public enum Mode
        {
            Static,
            Context,
            Dynamic
        }
        
        // =======================================================================
        private void Awake()
        {
            Instance = this;
        }

        public void Show(UITooltipContent ctx)
        {
            _text.text = ctx._content;
            _onShow.Invoke(ctx._content);

            switch (_mode)
            {
                case Mode.Static:
                    break;
                case Mode.Context:
                {
                    transform.position = ctx._anchor.GetValueOrDefault(ctx.transform).position;
                } break;
                case Mode.Dynamic:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void Hide()
        {
            _onHide.Invoke();
        }
    }
}