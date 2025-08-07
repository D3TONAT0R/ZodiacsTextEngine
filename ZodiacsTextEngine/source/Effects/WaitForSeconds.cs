using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class WaitForSeconds : Effect
	{
		public float delay;

		public WaitForSeconds(float seconds)
		{
			delay = seconds;
		}

		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.Wait((int)(delay * 1000));
		}
	}
}