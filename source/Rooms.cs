using System;
using System.Collections.Generic;
using System.Linq;
using static ZodiacsTextEngine.TextEngine;

namespace ZodiacsTextEngine
{
	public static class Rooms
	{
		public static Room StartRoom { get; internal set; }

		private static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

		public static Dictionary<string, Room>.ValueCollection All => rooms.Values;

		public static void RegisterRoom(string name, Room room)
		{
			rooms.Add(name, room);
		}

		public static Room GetRoom(string name)
		{
			return rooms[name];
		}

		public static bool Exists(string name)
		{
			return rooms.ContainsKey(name);
		}

		public static bool LoadRooms(RoomDataLoader loader)
		{
			if(DebugMode) Interface.Header("GAME FILE LOAD");
			try
			{
				var files = loader.Invoke();
				if(DebugMode) Interface.Text($"Loading {files.Count()} room files ...");
				foreach(var file in files)
				{
					if(DebugMode)
					{
						try
						{
							var room = RoomFileParser.Parse(file.roomName, file.data);
							RegisterRoom(file.roomName, room);
						}
						catch(Exception e)
						{
							Interface.LogError("Failed to load room file: " + e.Message);
						}
					}
					else
					{
						var room = RoomFileParser.Parse(file.roomName, file.data);
						RegisterRoom(file.roomName, room);
					}
				}
			}
			catch(Exception e)
			{
				Interface.LogError(e.Message);
				return false;
			}
			Interface.VerticalSpace();
			return true;
		}

		public static void ValidateRooms()
		{
			bool headerPrinted = false;
			foreach(var room in rooms.Values)
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
