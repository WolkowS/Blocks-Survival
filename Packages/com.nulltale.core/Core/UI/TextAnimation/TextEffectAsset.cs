using SoCreator;
using TMPro;
using UnityEngine;

namespace CoreLib.TextAnimation
{
    [SoCreate(true)]
    public abstract class TextEffectAsset : ScriptableObject
    {
        public string _linkTag;

        public abstract bool ColorEffect  { get; }
        public abstract bool VertexEffect { get; }

        // =======================================================================
        public virtual void Setup(in TMP_LinkInfo linkInfo) { }

        public virtual void ApplyVertex(in TMP_CharacterInfo charInfo, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d) { }

        public virtual void ApplyColor(in TMP_CharacterInfo charInfo, ref Color32 a, ref Color32 b, ref Color32 c, ref Color32 d) { }
    }
}