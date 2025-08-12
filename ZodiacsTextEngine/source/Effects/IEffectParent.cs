using System.Collections.Generic;

namespace ZodiacsTextEngine.Effects
{
	public interface IEffectParent
	{
		IEnumerable<Effect> GetChildEffects();
	}
}