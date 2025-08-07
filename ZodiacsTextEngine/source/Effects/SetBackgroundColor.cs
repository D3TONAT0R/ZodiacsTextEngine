using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class SetBackgroundColor : Effect
	{
		public ConsoleColor color;

		public SetBackgroundColor(ConsoleColor color)
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
			return new SetBackgroundColor(RoomParser.ParseConsoleColor(ctx.parserContext, args[0], ctx.startLinePos));
		}
	}
}