using CoreLib.Values;

namespace CoreLib.Scripting
{
    public class IntCompare : Comparison<int>
    {
        public void Invoke(GvInt val)
        {
            Invoke(val.Value);
        }
    }
}