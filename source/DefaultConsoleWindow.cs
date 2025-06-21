using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZodiacsTextEngine
{
	public class DefaultConsoleWindow : ITextInterface
	{
		public const ConsoleColor DEFAULT_BACKGROUND_COLOR = ConsoleColor.Black;
		public const ConsoleColor MAIN_COLOR = ConsoleColor.Gray;
		public const ConsoleColor HINT_COLOR = ConsoleColor.DarkGray;
		public const int DEFAULT_WIDTH = 120;

		public bool DebugMode { get; private set; }

		public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
		public ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }

		public virtual void Initialize(bool debug)
		{
			DebugMode = debug;
			Console.CancelKeyPress += (sender, e) => e.Cancel = true;
		}

		public virtual void OnLoadError()
		{
			Text("An error occurred while loading the game files. Press any key to exit.", ConsoleColor.DarkRed);
			Console.ReadKey();
		}

		public virtual void PrintConsoleInfo()
		{
			Header("CONSOLE INFO");
			Console.WriteLine($"Window size: {Console.WindowWidth} x {Console.WindowHeight}");
			Console.WriteLine($"Buffer size: {Console.BufferWidth} x {Console.BufferHeight}");
			Console.WriteLine("Console colors:");
			var colors = Enum.GetValues(typeof(ConsoleColor));
			for(int i = 0; i < colors.Length; i++)
			{
				var color = colors.GetValue(i);
				bool isBlack = (ConsoleColor)color == ConsoleColor.Black;
				if(isBlack) Console.BackgroundColor = ConsoleColor.DarkGray;
				Console.ForegroundColor = (ConsoleColor)color;
				Console.Write($" ██  {color} ".PadRight(20));
				Console.ResetColor();
				if((i + 1) % 4 == 0) Console.WriteLine();
			}
			Console.WriteLine();
		}

		public virtual void OnDebugInfo()
		{

		}

		public virtual void Clear()
		{
			ResetColors();
			Console.Clear();
		}

		public virtual async Task<Choice> RequestChoice(Room room)
		{
			Choice choice = null;
			do
			{
				LineBreak();
				Console.Write("> ");
				string input = await ReadInput();

				if(DebugMode)
				{
					if(input == "?")
					{
						ListChoices(room.choices);
						continue;
					}
				}

				choice = room.GetChoice(input);
				if(choice == null)
				{
					Text("Unknown option: " + input);
				}
			}
			while(choice == null);

			return choice;
		}

		public virtual Task<string> ReadInput()
		{
			int posX = Console.CursorLeft;
			int posY = Console.CursorTop;
			while(true)
			{
				var line = Console.ReadLine();
				if(!string.IsNullOrWhiteSpace(line))
				{
					return Task.FromResult(line);
				}
				else
				{
					Console.CursorLeft = posX;
					Console.CursorTop = posY;
				}
			}
		}

		public virtual void Text(string text, ConsoleColor? color = null)
		{
			if(color.HasValue) Console.ForegroundColor = color.Value;
			Write(text, true);
			Console.ForegroundColor = MAIN_COLOR;
		}

		public virtual void LineBreak()
		{
			var foregroundColor = Console.ForegroundColor;
			var backgroundColor = Console.BackgroundColor;
			ResetColors();
			Console.WriteLine();
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
		}

		public virtual void Write(string text, bool lineBreak, bool wordWrap = true)
		{

			if(wordWrap)
			{
				int xPos = Console.CursorLeft;
				var lines = TextUtility.GetWordWrappedLines(text, DEFAULT_WIDTH, ref xPos);
				//Console.CursorLeft = xPos;
				for(int i = 0; i < lines.Length; i++)
				{
					var line = lines[i];
					Console.Write(line);
					if(i < lines.Length - 1 || lineBreak) LineBreak();
				}
			}
			else
			{
				Console.Write(text);
				if(lineBreak) LineBreak();
			}
		}

		public virtual void Header(string title)
		{
			Text("-------------------------");
			if(!string.IsNullOrEmpty(title))
			{
				Text(title);
				Text("-------------------------");
			}
		}

		public virtual void Hint(string text)
		{
			Text(text, HINT_COLOR);
		}

		public virtual void VerticalSpace(int count = 1)
		{
			for(int i = 0; i < count; i++)
			{
				LineBreak();
			}
		}

		public virtual Task WaitForInput(bool printLine)
		{
			const string message = "[Press any key to continue]";
			if(printLine) Hint(message);
			Console.ReadKey(true);
			Console.CursorTop--;
			Text(new string(' ', message.Length));
			Console.CursorTop--;
			return Task.CompletedTask;
		}

		public virtual Task Wait(int milliseconds)
		{
			Thread.Sleep(milliseconds);
			return Task.CompletedTask;
		}

		public virtual void ListChoices(List<Choice> choices)
		{
			var sb = new StringBuilder("You can ");
			for(int i = 0; i < choices.Count; i++)
			{
				if(i > 0) sb.Append((i < choices.Count - 1) ? ", " : " or ");
				sb.Append(choices[i].prompt.ToUpper());
			}
			sb.Append('.');
			Text(sb.ToString(), HINT_COLOR);
		}

		public virtual async Task OnGameOver(string text)
		{
			Text("---------");
			Text("GAME OVER");
			Text("---------");
			await WaitForInput(false);

			await TextEngine.StartGame();
		}

		public virtual void ResetColors()
		{
			Console.BackgroundColor = DEFAULT_BACKGROUND_COLOR;
			Console.ForegroundColor = MAIN_COLOR;
		}

		public virtual void LogWarning(string message)
		{
			Text(message, ConsoleColor.Yellow);
		}

		public virtual void LogError(string message)
		{
			Text(message, ConsoleColor.DarkRed);
		}
	}
}
