using ZodiacsTextEngine;

namespace ZodiacsTextConsole
{
	internal class Program
	{
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

		private static async Task Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.Black;
			ParseArguments(args, out var inputPath, out var debug, out string startRoom, out bool showMenu);
			if(showMenu)
			{
				await MainMenu();
			}
			else if(inputPath != null)
			{
				try
				{
					await Run(inputPath, debug, startRoom);
				}
				catch(Exception e)
				{
					Console.ResetColor();
					Console.WriteLine("An error occurred while running the game:");
					Console.WriteLine(e.Message);
					if(debug)
					{
						Console.WriteLine(e.StackTrace);
					}
					Console.WriteLine("Press any key to exit.");
					Console.ReadKey(true);
				}
			}
			else
			{
				Console.WriteLine("No content path specified. Use --content <path> to specify a directory or zip file containing the game data " +
					"or use --menu to show the main menu.");
				Console.WriteLine("Use -h or --help for usage information.");
			}
		}

		private static async Task Run(string inputPath, bool debug, string startRoom = "start")
		{
			await TextEngine.Initialize(new DefaultConsoleWindow(), new StandardGameDataLoader(inputPath, startRoom), debug);
			await TextEngine.StartGame();
		}

		private static async Task MainMenu()
		{
			Console.WriteLine();
			foreach(var line in TITLE_ART)
			{
				Console.WriteLine(line);
			}
			Console.WriteLine();
			if(!Directory.Exists("content"))
			{
				Directory.CreateDirectory("content");
			}
			List<string> contents = new();
			contents.AddRange(Directory.GetFiles("content", "*.zip").Select(Path.GetFileName)!);
			contents.AddRange(Directory.GetDirectories("content").Select(Path.GetFileName)!);
			if(contents.Count == 0)
			{
				Console.WriteLine("No content found in the 'content' directory. Add zip files or directories containing game data to the 'content' folder " +
					"to have them show here.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadKey(true);
				return;
			}
			Console.WriteLine("CONTENT SELECTION:");
			Console.WriteLine();
			for(int i = 0; i < contents.Count; i++)
			{
				Console.WriteLine($"  {i + 1}. {contents[i]}");
			}
			Console.WriteLine();
			Console.WriteLine("Enter the number of the content to load, or type 'exit' to quit:");
			do
			{
				string? input = Console.ReadLine()?.Trim();
				if(input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				if(int.TryParse(input, out int index) && index > 0 && index <= contents.Count)
				{
					string selectedContent = contents[index - 1];
					string contentPath = Path.Combine("content", selectedContent);
					Console.WriteLine($"Loading content: {selectedContent}");
					await Run(contentPath, false);
					return;
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid number or 'exit' to quit.");
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
					Console.WriteLine("  --debug            Enable debug mode");
					Console.WriteLine("  --content <path>    Specify the content path (zip file or directory)");
					Console.WriteLine("  --startroom <name>  Specify the starting room name");
					Console.WriteLine("  -h, --help         Show this help message");
				}
				else
				{
					Console.WriteLine($"Unknown argument: {args[i]}");
				}
			}
		}
	}
}
