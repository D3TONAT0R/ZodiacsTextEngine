using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class Exit : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.Exit();
		}

		[EffectParser("EXIT")]
		public static Exit Parse(EffectParseContext ctx)
		{
			return new Exit();
		}
	}
}
