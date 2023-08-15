namespace CoreLib
{
    public interface IValueProvider
    {
    }
    
    public interface IValueProvider<out TValue> : IValueProvider
    {
        TValue Value { get; }
    }
}