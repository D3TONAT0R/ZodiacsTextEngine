using System.Reflection;
using ZodiacsTextEngine;

namespace ZodiacsTextConsole
{
	internal class Program
	{
		class StoryEntry
		{
			public string path;
			public StoryMetadata metadata;

			public StoryEntry(string path)
			{
				this.path = Path.GetFileName(path);
				metadata = metadataLoader.FetchMetadataFrom(path);
			}
		}

		private const string CONTENT_SUBDIR = "stories";

		private static readonly string[] TITLE_ART =
		[
			@"  ______         _ _             _______        _   ______             _            ",
			@" |___  /        | (_)           |__   __|      | | |  ____|           (_)           ",
			@"    / / ___   __| |_  __ _  ___ ___| | _____  _| |_| |__   _ __   __ _ _ _ __   ___ ",
			@"   / / / _ \ / _` | |/ _` |/ __/ __| |/ _ \ \/ / __|  __| | '_ \ / _` | | '_ \ / _ \",
			@"  / /_| (_) | (_| | | (_| | (__\__ \ |  __/>  <| |_| |____| | | | (_| | | | | |  __/",
			@" /_____\___/ \__,_|_|\__,_|\___|___/_|\___/_/\_\\__|______|_| |_|\__, |_|_| |_|\___|",
			@"                                                                  __/ |             ",
			@"                                                                 |___/              "
		];

		private static bool debugMode = false;

		private static ContentLoader metadataLoader = new StandardContentLoader(null, null);

		private static async Task Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.Black;

			ParseArguments(args, out var inputPath, out debugMode, out string startRoom, out bool showMenu);
			if(showMenu)
			{
				await MainMenu();
			}
			else if(inputPath != null)
			{
				await Run(inputPath, startRoom);
			}
			else
			{
				Console.WriteLine("No content path specified. Use --content <path> to specify a directory or zip file containing the game data " +
					"or use --menu to show the main menu.");
				Console.WriteLine("Use -h or --help for usage information.");
			}
		}

		private static async Task Run(string inputPath, string startRoom = "start")
		{
			try
			{
				Console.Title = Path.GetFileNameWithoutExtension(inputPath);
				await TextEngine.Initialize(new DefaultConsoleWindow(), new StandardContentLoader(inputPath, startRoom), debugMode);
				await TextEngine.Start();
			}
			catch(Exception e)
			{
				Console.ResetColor();
				Console.WriteLine("An error occurred while running the game:");
				Console.WriteLine(e.Message);
				if(debugMode)
				{
					Console.WriteLine(e.StackTrace);
				}
				Console.WriteLine("Press any key to exit.");
				Console.ReadKey(true);
			}

		}

		private static async Task MainMenu()
		{
			Console.Title = "Zodiacs Text Console";
			Console.WriteLine();
			foreach(var line in TITLE_ART)
			{
				Console.WriteLine(line);
			}
			Console.WriteLine();
			if(debugMode)
			{
				Console.WriteLine("Debug mode is active.");
				Console.WriteLine();
			}
			List<StoryEntry> stories = new();
			if(Directory.Exists(CONTENT_SUBDIR))
			{
				stories.AddRange(Directory.GetFiles(CONTENT_SUBDIR, "*.zip").Select(zip => new StoryEntry(zip)));
				stories.AddRange(Directory.GetDirectories(CONTENT_SUBDIR).Select(dir => new StoryEntry(dir)));
			}
			if(stories.Count == 0)
			{
				Console.WriteLine($"No content found in the '{CONTENT_SUBDIR}' directory. Add zip files or directories containing story files to the " +
					$"'{CONTENT_SUBDIR}' folder to have them listed here.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadKey(true);
				return;
			}
			Console.WriteLine("STORY SELECTION:");
			Console.WriteLine();
			for(int i = 0; i < stories.Count; i++)
			{
				Console.WriteLine($"  {i + 1}. {stories[i].metadata.Title}");
			}
			Console.WriteLine();
			Console.WriteLine("Enter the number of the story to play, or type 'quit' to quit:");
			do
			{
				string? input = Console.ReadLine()?.Trim();
				if(input == null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				if(int.TryParse(input, out int index) && index > 0 && index <= stories.Count)
				{
					StoryEntry selectedContent = stories[index - 1];
					string storyPath = Path.Combine(CONTENT_SUBDIR, selectedContent.path);
					Console.WriteLine($"Loading story: {selectedContent.metadata.Title}");
					await Run(storyPath);
					return;
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid number or 'quit' to quit.");
				}
			}
			while(true);
		}

		private static void ParseArguments(string[] args, out string? inputPath, out bool debug, out string startRoom, out bool showMenu)
		{
			inputPath = null;
			debug = false;
			startRoom = "start";
			showMenu = false;
			for(int i = 0; i < args.Length; i++)
			{
				if(args[i] == "--debug")
				{
					debug = true;
				}
				else if(args[i] == "--content" && i + 1 < args.Length)
				{
					inputPath = args[++i];
				}
				else if(args[i] == "--startroom" && i + 1 < args.Length)
				{
					startRoom = args[++i];
				}
				else if(args[i] == "--menu")
				{
					showMenu = true;
				}
				else if(args[i] == "-h" || args[i] == "--help")
				{
					Console.WriteLine("Usage: ZodiacsTextConsole [options]");
					Console.WriteLine("Options:");
					Console.WriteLine("  --menu             Show main menu with content selection");
					Console.WriteLine("  --content <path>   Specify the content path (zip file or directory)");
					Console.WriteLine("  --startroom <name> Specify the starting room name");
					Console.WriteLine("  --debug            Enable debug mode");
					Console.WriteLine("  -h, --help         Show this help message");
				}
				else
				{
					Console.WriteLine($"Unknown argument: {args[i]}");
				}
			}
		}

		private static Assembly? ResolveAssembly(object? sender, ResolveEventArgs eventArgs)
		{
			string assemblyName = new AssemblyName(eventArgs.Name).Name;
			string assemblyPath = Path.Combine(AppContext.BaseDirectory, "libs", $"{assemblyName}.dll");
			if(File.Exists(assemblyPath))
			{
				return Assembly.LoadFrom(assemblyPath);
			}

			return null;
		}
	}
}
