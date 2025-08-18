using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class Return : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await GameSession.Current.GoToRoom(GameSession.Current.currentRoom);
		}

		[EffectParser("RETURN")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new Return();
		}
	}
}