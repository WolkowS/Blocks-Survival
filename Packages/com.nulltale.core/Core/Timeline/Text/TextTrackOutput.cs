using TMPro;

namespace CoreLib.Timeline
{
    public class TextTrackOutput : Singleton<TextTrackOutput>
    {
        public TMP_Text Text => GetComponent<TMP_Text>();
    }
}