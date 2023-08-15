namespace CoreLib.Scripting
{
    public class BoolContainer : ValueContainer<bool>
    {
        public void Toggle() => Value = !Value;
        
        public void SetTrue()  => Value = true;
        public void SetFalse() => Value = false;
    }
}