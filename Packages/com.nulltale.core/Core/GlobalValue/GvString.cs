namespace CoreLib.Values
{
    public class GvString : GlobalValue<string>
    {
        public override string ToString()
        {
            return Value;
        }
        
        internal override string Serialize()
        {
            return Value;
        }

        internal override void Deserialize(string data)
        {
            Value = data;
        }
    }
}