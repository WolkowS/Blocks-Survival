using System;
using System.Text;

namespace CoreLib.Values
{
    public class GvTime : GvFloat
    {
        public override string ToString()
        {
            var sb   = new StringBuilder();
            var time = TimeSpan.FromSeconds(Value);
            if (time.Hours > 0)
                sb.Append(time.Hours).Append(':');
            if (time.Minutes > 0)
                sb.Append(time.Minutes.ToString("00")).Append(':');

            sb.Append(time.Seconds).Append(':');
            sb.Append(time.Milliseconds.ToString("000"));

            return sb.ToString();
        }

    }
}