using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ValueAsset : PlayableAsset
    {
        [InplaceField(nameof(ValueBehaviour.m_Value))]
        public ValueBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<ValueBehaviour>.Create(graph, m_Template);
            return playable;
        }
    }
}