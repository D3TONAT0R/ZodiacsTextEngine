using System.Collections.Generic;

namespace ZodiacsTextEngine.Effects
{
	public interface ITextEffect
	{
		IEnumerable<string> GetTextStrings();
	}
}