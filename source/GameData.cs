using System;
using System.Collections.Generic;
using System.Text;
using static ZodiacsTextEngine.Functions;

namespace ZodiacsTextEngine.source
{
	public class GameData
	{
		public Dictionary<string, Room> Rooms { get; } = new Dictionary<string, Room>();

		public string StartRoomName { get; private set; }

		public Room StartRoom
		{
			get
			{
				if(StartRoomName == null)
				{
					throw new NullReferenceException($"Start room not set.");
				}
				if(Rooms.TryGetValue(StartRoomName, out var room))
				{
					return room;
				}
				else
				{
					throw new NullReferenceException($"Could not find start room $'{StartRoomName}'.");
				}
			}
		}

		public List<string> VariableNames { get; } = new List<string>();

		public Dictionary<string, FunctionDelegate> Functions { get; } = new Dictionary<string, FunctionDelegate>();

		public GameData(Dictionary<string, Room> rooms, string startRoomName, List<string> variableNames)
		{
			Rooms = rooms;
			StartRoomName = startRoomName;
			VariableNames = variableNames ?? VariableNames;
		}

		public GameData()
		{

		}

		public void SetStartRoom(string roomName)
		{
			StartRoomName = roomName;
		}
	}
}
