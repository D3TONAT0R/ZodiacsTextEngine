using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static ZodiacsTextEngine.TextEngine;

namespace ZodiacsTextEngine
{
	public class StandardContentLoader : ContentLoader
	{
		protected struct DataFile
		{
			public string fileName;
			public string content;

			public DataFile(string fileName, string content)
			{
				this.fileName = fileName;
				this.content = content;
			}
		}

		public const string ROOM_FILE_EXT = "room";
		public const string FUNC_FILE_EXT = "funcs";

		protected readonly string rootDirectory;
		protected readonly string startRoomName;

		protected ZipArchive zipArchive;

		public StandardContentLoader(string rootDirectory, string startRoomName = "start")
		{
			this.rootDirectory = rootDirectory;
			this.startRoomName = startRoomName;
		}

		protected override void Begin()
		{
			if(DebugMode) Interface.Header("CONTENT LOAD");
			if(rootDirectory != null)
			{
				//Check if the root directory is a zip file
				if(Path.GetFileName(rootDirectory).EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					if(DebugMode)
					{
						Interface.Text($"Loading story content from zip file '{rootDirectory}' ...");
						Interface.LineBreak();
					}
					zipArchive = ZipFile.OpenRead(rootDirectory);
				}
				else
				{
					if(DebugMode)
					{
						Interface.Text($"Loading story content from directory '{rootDirectory}' ...");
						Interface.LineBreak();
					}
				}
			}
		}

		protected override IEnumerable<(string, Functions.FunctionDelegate)> LoadFunctions()
		{
			var funcFiles = LoadAllFiles(FUNC_FILE_EXT);
			if(funcFiles.Length == 0) yield break;

			if(DebugMode) Interface.Text($"Compiling {funcFiles.Length} function files ...");
			var compiler = new FunctionCompiler();
			foreach(var funcFile in funcFiles)
			{
				compiler.AddFunctionSourcesFromFile(funcFile.content);
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
			var roomFiles = LoadAllFiles(ROOM_FILE_EXT);
			if(DebugMode) Interface.Text($"Loading {roomFiles.Length} room files ...");
			foreach(var file in roomFiles)
			{
				Room room;
				try
				{
					room = Room.Parse(story, file.fileName, file.content);
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

		protected override void Complete()
		{
			if(zipArchive != null)
			{
				zipArchive.Dispose();
				zipArchive = null;
			}
		}

		protected DataFile[] LoadAllFiles(string extension)
		{
			var files = new List<DataFile>();
			if(zipArchive != null)
			{
				foreach(var entry in zipArchive.Entries.Where(e => e.FullName.ToLower().EndsWith("." + extension)))
				{
					//Load file as text
					using(var stream = entry.Open())
					{
						using(var reader = new StreamReader(stream))
						{
							string content = reader.ReadToEnd();
							files.Add(new DataFile(entry.FullName, content));
						}
					}
				}
			}
			else
			{
				foreach(var fileName in Directory.GetFiles(rootDirectory, "*." + extension, SearchOption.AllDirectories))
				{
					files.Add(new DataFile(fileName, File.ReadAllText(fileName)));
				}
			}
			return files.ToArray();
		}
	}
}