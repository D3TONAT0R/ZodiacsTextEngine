using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

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

		[EffectParser("GAME_OVER")]
		public static GameOver Parse(EffectParseContext ctx)
		{
			return new GameOver(ctx.content);
		}
	}
}