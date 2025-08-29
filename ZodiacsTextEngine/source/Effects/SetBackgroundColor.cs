using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class SetBackgroundColor : Effect
	{
		public Color color;

		public SetBackgroundColor(Color color)
		{
			this.color = color;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.BackgroundColor = color;
			return Task.CompletedTask;
		}

		[EffectParser("BACKGROUND")]
		public static SetBackgroundColor Parse(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new SetBackgroundColor(RoomParser.ParseColor(ctx.parserContext, args[0], ctx.startLinePos));
		}
	}
}