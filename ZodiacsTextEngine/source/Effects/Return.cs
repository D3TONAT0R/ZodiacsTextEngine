using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class Return : Effect
	{
		public override async Task Execute(EffectGroup g)
		{
			await Session.Current.GoToRoom(Session.Current.currentRoom);
		}

		[EffectParser("RETURN")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new Return();
		}
	}
}