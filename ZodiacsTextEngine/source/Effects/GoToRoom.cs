using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class GoToRoom : Effect
	{
		string nextRoomName;

		public GoToRoom(string nextRoomName)
		{
			this.nextRoomName = nextRoomName;
		}

		public override async Task Execute(EffectGroup g)
		{
			await GameSession.Current.GoToRoom(nextRoomName);
		}

		public override LogMessage Validate(string site)
		{
			if(!Rooms.Exists(nextRoomName)) return LogMessage.Error(site, "Nonexisting room referenced: " + nextRoomName);
			return null;
		}

		[EffectParser("GOTO")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new GoToRoom(ctx.content.Trim());
		}
	}
}