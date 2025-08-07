using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class ClearOutput : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.Clear();
			return Task.CompletedTask;
		}

		[EffectParser("CLEAR_OUTPUT")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new ClearOutput();
		}
	}
}