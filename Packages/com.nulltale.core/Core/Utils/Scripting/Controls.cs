namespace CoreLib
{
    public interface IToggle
    {
        public bool IsOn { get; }
        
        void On();
        void Off();
    }
    
    public interface ISlider
    {
        public float Lerp { get; set; }
    }
    
    public interface IActivator
    {
        void Apply();
    }
}