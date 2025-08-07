using System;
using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class Function : Effect
	{
		public Action action;

		public Function(Action action)
		{
			this.action = action;
		}

		public override Task Execute(EffectGroup g)
		{
			action.Invoke();
			return Task.CompletedTask;
		}
	}
}