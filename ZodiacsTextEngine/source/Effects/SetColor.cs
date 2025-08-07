using System;
using System.Threading.Tasks;

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
	}
}