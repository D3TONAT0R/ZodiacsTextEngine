using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class Space : Effect
	{
		int count;

		public Space(int count)
		{
			this.count = count;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.VerticalSpace(count);
			return Task.CompletedTask;
		}
	}
}