using System;
using System.Collections.Generic;
using static ZodiacsTextEngine.Functions;

namespace ZodiacsTextEngine
{
	public class Story
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
					throw new NullReferenceException($"Could not find start room '{StartRoomName}'.");
				}
			}
		}

		public List<string> VariableNames { get; } = new List<string>();

		internal Dictionary<string, FunctionDelegate> Functions { get; } = new Dictionary<string, FunctionDelegate>();

		public ConsoleColor DefaultForegroundColor { get; set; } = ConsoleColor.Gray;
		public ConsoleColor DefaultBackgroundColor { get; set; } = ConsoleColor.Black;

		public ConsoleColor? HighlightForegroundColor { get; set; } = ConsoleColor.Yellow;
		public ConsoleColor? HighlightBackgroundColor { get; set; } = null;

		public ConsoleColor? HintForegroundColor { get; set; } = ConsoleColor.DarkGray;
		public ConsoleColor? HintBackgroundColor { get; set; } = null;

		public ConsoleColor? InputForegroundColor { get; set; } = null;
		public ConsoleColor? InputBackgroundColor { get; set; } = null;

		public Story(Dictionary<string, Room> rooms, string startRoomName, List<string> variableNames)
		{
			Rooms = rooms;
			StartRoomName = startRoomName;
			VariableNames = variableNames ?? VariableNames;
		}

		public Story()
		{

		}

		public void SetStartRoom(string roomName)
		{
			StartRoomName = roomName;
		}
	}
}
