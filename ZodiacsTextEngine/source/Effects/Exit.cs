using System;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class Exit : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Environment.Exit(0);
			return Task.CompletedTask;
		}
	}
}
