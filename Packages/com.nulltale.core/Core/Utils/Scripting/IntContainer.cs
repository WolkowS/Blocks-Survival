namespace CoreLib.Scripting
{
    public class IntContainer : ValueContainer<int>
    {
        public void Add(int val) => Value += val;
        
        public void Increment() => Value ++;
        public void Decrement() => Value --;
    }
}