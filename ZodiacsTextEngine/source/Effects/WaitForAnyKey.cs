using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class WaitForAnyKey : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.WaitForInput(true);
		}

		[EffectParser("WAIT")]
		public static WaitForAnyKey Parse(EffectParseContext ctx)
		{
			return new WaitForAnyKey();
		}
	}
}