using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class SetColor : Effect
	{
		public ConsoleColor color;

		public SetColor(ConsoleColor color)
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
			return new SetColor(RoomParser.ParseConsoleColor(ctx.parserContext, args[0], ctx.startLinePos));
		}
	}
}