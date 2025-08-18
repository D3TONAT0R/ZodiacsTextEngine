using System;
using System.Collections.Generic;
using System.IO;
using static ZodiacsTextEngine.TextEngine;

namespace ZodiacsTextEngine
{
	public class StandardGameDataLoader : GameDataLoader
	{
		public const string ROOM_FILE_EXT = "txt";
		public const string FUNC_FILE_EXT = "funcs";

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

		protected override IEnumerable<(string, Functions.FunctionDelegate)> LoadFunctions()
		{
			var funcFiles = Directory.GetFiles(rootDirectory, "*." + FUNC_FILE_EXT, SearchOption.AllDirectories);
			if(funcFiles.Length == 0) yield break;

			if(DebugMode) Interface.Text($"Compiling {funcFiles.Length} function files ...");
			var compiler = new FunctionCompiler();
			foreach(var funcFile in funcFiles)
			{
				var lines = File.ReadAllLines(funcFile);
				compiler.AddFunctionSourcesFromFile(lines);
			}
			if(compiler.FunctionSourceCount == 0)
			{
				if(DebugMode) Interface.Text("No functions found in the function files.");
				yield break;
			}
			var compiled = compiler.CompileFunctions();
			foreach(var func in compiled)
			{
				if(DebugMode) Interface.Text($"Successfully compiled function '{func.Item1}'");
				yield return func;
			}
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