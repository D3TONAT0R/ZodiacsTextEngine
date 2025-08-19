using ZodiacsTextEngine;

namespace ZodiacsTextConsole
{
	internal class Program
	{
		private static async Task Main(string[] args)
		{
			ParseArguments(args, out var inputPath, out var debug, out string startRoom);
			if(inputPath != null)
			{
				await TextEngine.Initialize(new DefaultConsoleWindow(), new StandardGameDataLoader(inputPath, startRoom), debug);
				await TextEngine.StartGame();
			}
		}

		private static void ParseArguments(string[] args, out string? inputPath, out bool debug, out string startRoom)
		{
			inputPath = null;
			debug = false;
			startRoom = null;
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
