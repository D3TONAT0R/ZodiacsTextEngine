using System;

namespace ZodiacsTextEngine
{
	public static class Rooms
	{
		public static Room GetRoom(string name)
		{
			return TextEngine.GameData.Rooms[name];
		}

		public static bool Exists(string name)
		{
			return TextEngine.GameData.Rooms.ContainsKey(name);
		}

		public static void Validate(GameData gameData)
		{
			bool headerPrinted = false;
			foreach(var room in gameData.Rooms.Values)
			{
				if(!RoomValidator.Validate(room, out var log))
				{
					if(!headerPrinted)
					{
						TextEngine.Interface.BackgroundColor = ConsoleColor.DarkRed;
						TextEngine.Interface.ForegroundColor = ConsoleColor.White;
						TextEngine.Interface.Text("Room validation has found some problems:");
						TextEngine.Interface.ResetColors();
						headerPrinted = true;
					}
					foreach(var msg in log)
					{
						msg.Print();
					}
					log.AddRange(log);
				}
			}
		}
	}
}
