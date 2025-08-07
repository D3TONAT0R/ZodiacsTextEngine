using System;
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

		protected override void LoadContent(ref bool success)
		{
			if(DebugMode) Interface.Header("GAME FILE LOAD");
			try
			{
				var roomFiles = Directory.GetFiles(rootDirectory, "*." + ROOM_FILE_EXT, SearchOption.AllDirectories);
				if(DebugMode) Interface.Text($"Loading {roomFiles.Length} room files ...");
				foreach(var file in roomFiles)
				{
					try
					{
						var room = Room.FromFile(file);
						AddRoom(room);
					}
					catch(Exception e)
					{
						if(DebugMode)
						{
							Interface.LogError("Failed to load room file: " + e.Message);
							Interface.VerticalSpace();
						}
						else throw;
					}
				}
				if(startRoomName != null)
				{
					SetStartRoom(startRoomName);
				}

				var varFiles = Directory.GetFiles(rootDirectory, "*." + VARS_FILE_EXT, SearchOption.AllDirectories);
				if(DebugMode && varFiles.Length > 0) Interface.Text($"Loading {varFiles.Length} variable files ...");
				foreach(var file in varFiles)
				{
					try
					{
						var variables = File.ReadAllLines(file);
						foreach(var variable in variables)
						{
							if(!string.IsNullOrWhiteSpace(variable))
							{
								AddVariable(variable.Trim());
							}
						}
					}
					catch(Exception e)
					{
						if(DebugMode)
						{
							Interface.LogError("Failed to load variable file: " + e.Message);
							Interface.VerticalSpace();
						}
						else throw;
					}
				}
			}
			catch(Exception e)
			{
				Interface.LogError(e.Message);
				success = false;
			}
			Interface.VerticalSpace();
		}
	}
}