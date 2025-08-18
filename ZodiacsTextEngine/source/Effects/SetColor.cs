using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class SetColor : Effect
	{
		public Color color;

		public SetColor(Color color)
		{
			this.color = color;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.ForegroundColor = color;
			return Task.CompletedTask;
		}

		[EffectParser("COLOR")]
		public static SetColor Parse(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new SetColor(RoomParser.ParseColor(ctx.parserContext, args[0], ctx.startLinePos));
		}
	}
}