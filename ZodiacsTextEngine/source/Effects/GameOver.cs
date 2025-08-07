using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class GameOver : Effect
	{
		public string text;

		public GameOver(string text)
		{
			this.text = text;
		}

		public override async Task Execute(EffectGroup g)
		{
			await TextEngine.Interface.OnGameOver(text);
		}
	}
}