using System;
using System.Threading.Tasks;

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
	}
}