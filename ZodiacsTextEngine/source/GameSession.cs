using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public class GameSession
	{
		public static GameSession Current { get; private set; }

		public Variables variables = new Variables();
		public Room currentRoom = null;

		private GameSession()
		{

		}

		public static async Task StartNew()
		{
			Current = new GameSession();
			await Current.GoToRoom(TextEngine.GameData.StartRoom);
		}

		public async Task GoToRoom(string nextRoomName)
		{
			await GoToRoom(Rooms.GetRoom(nextRoomName));
		}

		public async Task GoToRoom(Room next)
		{
			//Execute OnExit event (if available)$
			if(currentRoom != null && currentRoom.onExit != null) await currentRoom.onExit.Execute();

			currentRoom = next;

			//Execute OnEnter event
			await currentRoom.onEnter.Execute();

			//List available choices & request user input until we leave this room
			//ConsoleWindow.ListChoices(currentRoom.choices);
			bool stayedInSameRoom;
			do
			{
				var lastRoom = currentRoom;
				await TextEngine.RequestChoice();
				stayedInSameRoom = lastRoom == currentRoom;
			}
			while(stayedInSameRoom);
		}
	}
}
