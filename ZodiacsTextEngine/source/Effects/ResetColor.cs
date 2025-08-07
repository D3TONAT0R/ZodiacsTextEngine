using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class ResetColor : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.ResetColors();
			return Task.CompletedTask;
		}
	}
}