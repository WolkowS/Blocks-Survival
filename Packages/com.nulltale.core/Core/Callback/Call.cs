using NaughtyAttributes;

namespace CoreLib
{
    public class Call : EventContainer
    {
        [Button("Invoke")]
        private void _invoke()
        {
            Invoke();
        }
    }
}