using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class WaitForAnyKey : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.WaitForInput(true);
		}
	}
}