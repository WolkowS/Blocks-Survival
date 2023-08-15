using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = "PlayerPrefs", menuName = Core.k_CoreModuleMenu + "PlayerPrefs")]
    public class PlayerPrefsValues : Core.Module<PlayerPrefsValues>
    {
        public const string k_FieldNamePrefsList = nameof(m_Values);

        public SoCollection<PlayerPrefsValue> m_Values;

        // =======================================================================
        public override void Init()
        {
            // write defaults at once, apply changes
            foreach (var prefsValue in m_Values)
                prefsValue.WriteDefault();

            PlayerPrefsValue.SaveChanges();
        }
    }
}