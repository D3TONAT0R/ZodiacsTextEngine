using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class WaitForSeconds : Effect
	{
		public float delay;

		public WaitForSeconds(float seconds)
		{
			delay = seconds;
		}

		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.Wait((int)(delay * 1000));
		}

		[EffectParser("DELAY")]
		public static WaitForSeconds Parse(EffectParseContext ctx)
		{
			return new WaitForSeconds(float.Parse(ctx.content.Trim()));
		}
	}
}