using System;
using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public static class Rooms
	{
		
		public static Dictionary<string, Room>.ValueCollection All => TextEngine.GameData.Rooms.Values;

		public static void RegisterRoom(string name, Room room)
		{
			TextEngine.GameData.Rooms.Add(name, room);
		}

		public static Room GetRoom(string name)
		{
			return TextEngine.GameData.Rooms[name];
		}

		public static bool Exists(string name)
		{
			return TextEngine.GameData.Rooms.ContainsKey(name);
		}

		public static void ValidateRooms(GameData gameData)
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
