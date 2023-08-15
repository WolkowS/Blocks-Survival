using CoreLib.Events;
using SoCreator;

namespace CoreLib.Midi
{
    [SoCreate]
    public class Note : GeVoid
    {
        public Signal<bool>  _pressed;
        public Signal<float> _velocity;
        
        // =======================================================================
        public void OnNote(float val)
        {
            if (val > 0f)
            {
                Invoke();
                _pressed.Value = true;
            }
            else
            {
                _pressed.Value = false;
            }
            _velocity.Value = val;
        }
    }
}