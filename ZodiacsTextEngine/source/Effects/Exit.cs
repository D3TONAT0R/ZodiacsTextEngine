using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class Exit : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Environment.Exit(0);
			return Task.CompletedTask;
		}

		[EffectParser("EXIT")]
		public static Exit Parse(EffectParseContext ctx)
		{
			return new Exit();
		}
	}
}
