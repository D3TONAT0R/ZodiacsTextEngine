using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class ClearOutput : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.Clear();
			return Task.CompletedTask;
		}
	}
}