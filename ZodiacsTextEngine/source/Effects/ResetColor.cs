using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class ResetColor : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.ForegroundColor = Color.DefaultForeground;
			TextEngine.Interface.BackgroundColor = Color.DefaultBackground;
			return Task.CompletedTask;
		}

		[EffectParser("RESET_COLOR")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new ResetColor();
		}
	}
}