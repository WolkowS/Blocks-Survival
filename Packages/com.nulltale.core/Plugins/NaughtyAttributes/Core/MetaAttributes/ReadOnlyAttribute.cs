using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ReadOnlyAttribute : MetaAttribute
	{
		public readonly bool InPlaymode;
		public readonly bool InEditor;
		
		// =======================================================================
		public ReadOnlyAttribute(bool inPlaymode = true, bool inEditor = true)
		{
			InPlaymode = inPlaymode;
			InEditor   = inEditor;
		}
	}
}
