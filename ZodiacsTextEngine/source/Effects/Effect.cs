using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public abstract class Effect
	{
		public ConditionalEffectBlock parent;

		public abstract Task Execute(EffectGroup g);

		public virtual LogMessage Validate(string site)
		{
			return null;
		}

		public override string ToString()
		{
			return GetType().Name;
		}


		public void ListEffectsAndChildren(List<Effect> effects)
		{
			effects.Add(this);
			if(this is IEffectParent p)
			{
				foreach(var c in p.GetChildEffects())
				{
					c.ListEffectsAndChildren(effects);
				}
			}
		}
	}
}
