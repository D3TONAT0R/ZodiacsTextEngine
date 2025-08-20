using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public class Session
	{
		public static Session Current { get; private set; }

		public Variables variables = new Variables();
		public Room currentRoom = null;

		private Session()
		{

		}

		public static async Task StartNew()
		{
			Current = new Session();
			await Current.GoToRoom(TextEngine.Story.StartRoom);
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
