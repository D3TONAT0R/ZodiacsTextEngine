using System;
using System.Collections.Generic;
using System.IO;
using static ZodiacsTextEngine.TextEngine;

namespace ZodiacsTextEngine
{
	public class StandardGameDataLoader : GameDataLoader
	{
		public const string ROOM_FILE_EXT = "txt";
		public const string VARS_FILE_EXT = "vars";

		protected readonly string rootDirectory;
		protected readonly string startRoomName;

		public StandardGameDataLoader(string rootDirectory, string startRoomName)
		{
			this.rootDirectory = rootDirectory;
			this.startRoomName = startRoomName;
		}

		protected override void Begin()
		{
			if(DebugMode) Interface.Header("GAME FILE LOAD");
		}

		protected override IEnumerable<Room> LoadRooms()
		{
			var roomFiles = Directory.GetFiles(rootDirectory, "*." + ROOM_FILE_EXT, SearchOption.AllDirectories);
			if(DebugMode) Interface.Text($"Loading {roomFiles.Length} room files ...");
			foreach(var file in roomFiles)
			{
				Room room;
				try
				{
					room = Room.FromFile(gameData, file);
				}
				catch(Exception e)
				{
					if(DebugMode)
					{
						Interface.LogError("Failed to load room file: " + e.Message);
						Interface.VerticalSpace();
						room = null;
					}
					else throw;
				}
				if(room != null) yield return room;
			}
			if(startRoomName != null)
			{
				SetStartRoom(startRoomName);
			}
			Interface.VerticalSpace();
		}
	}
}