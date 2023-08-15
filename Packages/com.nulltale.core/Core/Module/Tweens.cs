using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(Tweens), menuName = Core.k_CoreModuleMenu + nameof(Tweens))]
    public class Tweens : Core.Module
    {
        [SerializeField][Label("Init LeanTween")]
        private bool m_InitLeanTween;
        [SerializeField][Label("Tweens")]
        private int m_LTTweens    = 256;
        [SerializeField][Label("Sequences")]
        private int m_LTSequences = 64;
        
        [SerializeField][Label("Init DOTween")]
        private bool m_InitDOTween;
        [SerializeField] 
        private Optional<bool> m_Recycle;
        [SerializeField] 
        private Optional<bool> m_SafeMode;
        [SerializeField] 
        private Optional<LogBehaviour> m_Logging;
        [SerializeField][Label("Tweens")]
        private int m_DOTweens    = 256;
        [SerializeField][Label("Sequences")]
        private int m_DOSequences = 64;
        
        [SerializeField] [SoNested]
        private DOTweenSettings m_Settings;

        [SerializeField]
        private SoCollection<CurveAsset> m_Curve;
        
        [SerializeField]
        private SoCollection<AdrsCurveAsset> m_ADSR;

        public IReadOnlyDictionary<string, CurveAsset> Curves => m_Curve;
        public IReadOnlyDictionary<string, AdrsCurveAsset>   ADSRs  => m_ADSR;

        // =======================================================================
        public override void Init()
        {
            if (m_InitLeanTween)
            {
                // check or create lean tween on core
                var leanTween = Core.Instance.transform
                                    .GetChildren()
                                    .Select(n => n.GetComponent<LeanTween>())
                                    .FirstOrDefault(n => n != null);

                if (leanTween == null)
                {
                    var go = new GameObject("LeanTween");
                    go.transform.SetParent(Core.Instance.transform);

                    leanTween = go.AddComponent<LeanTween>();
                    go.transform.SetAsLastSibling();
                }
                
                LeanTween.init(m_LTTweens, m_LTSequences, leanTween.gameObject);
            }
            
            if (m_InitDOTween)
            {
                DOTween.Init(m_Recycle.GetValueOrDefault(), m_SafeMode.GetValueOrDefault(), m_Logging.GetValueOrDefault(), m_Settings)
                       .SetCapacity(m_DOTweens, m_DOSequences);
                
                DOTween.instance.transform.SetParent(Core.Instance.transform);
                DOTween.instance.transform.SetAsLastSibling();
            }

        }
    }
}