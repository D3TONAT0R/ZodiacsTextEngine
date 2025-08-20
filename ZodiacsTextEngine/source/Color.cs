using System;

namespace ZodiacsTextEngine
{
	public enum Color
	{
		Black,
		DarkBlue,
		DarkGreen,
		DarkCyan,
		DarkRed,
		DarkMagenta,
		DarkYellow,
		Gray,
		DarkGray,
		Blue,
		Green,
		Cyan,
		Red,
		Magenta,
		Yellow,
		White,

		DefaultForeground,
		DefaultBackground,
		HighlightForeground,
		HighlightBackground,
		HintForeground,
		HintBackground
	}

	public static class ColorExtensions
	{
		public static Color ToColor(this ConsoleColor color)
		{
			switch(color)
			{
				case ConsoleColor.Black: return Color.Black;
				case ConsoleColor.DarkBlue: return Color.DarkBlue;
				case ConsoleColor.DarkGreen: return Color.DarkGreen;
				case ConsoleColor.DarkCyan: return Color.DarkCyan;
				case ConsoleColor.DarkRed: return Color.DarkRed;
				case ConsoleColor.DarkMagenta: return Color.DarkMagenta;
				case ConsoleColor.DarkYellow: return Color.DarkYellow;
				case ConsoleColor.Gray: return Color.Gray;
				case ConsoleColor.DarkGray: return Color.DarkGray;
				case ConsoleColor.Blue: return Color.Blue;
				case ConsoleColor.Green: return Color.Green;
				case ConsoleColor.Cyan: return Color.Cyan;
				case ConsoleColor.Red: return Color.Red;
				case ConsoleColor.Magenta: return Color.Magenta;
				case ConsoleColor.Yellow: return Color.Yellow;
				case ConsoleColor.White: return Color.White;
				default: throw new ArgumentOutOfRangeException(nameof(color), color, null);
			}
		}

		public static ConsoleColor ToConsoleColor(this Color color)
		{
			switch(color)
			{
				case Color.Black: return ConsoleColor.Black;
				case Color.DarkBlue: return ConsoleColor.DarkBlue;
				case Color.DarkGreen: return ConsoleColor.DarkGreen;
				case Color.DarkCyan: return ConsoleColor.DarkCyan;
				case Color.DarkRed: return ConsoleColor.DarkRed;
				case Color.DarkMagenta: return ConsoleColor.DarkMagenta;
				case Color.DarkYellow: return ConsoleColor.DarkYellow;
				case Color.Gray: return ConsoleColor.Gray;
				case Color.DarkGray: return ConsoleColor.DarkGray;
				case Color.Blue: return ConsoleColor.Blue;
				case Color.Green: return ConsoleColor.Green;
				case Color.Cyan: return ConsoleColor.Cyan;
				case Color.Red: return ConsoleColor.Red;
				case Color.Magenta: return ConsoleColor.Magenta;
				case Color.Yellow: return ConsoleColor.Yellow;
				case Color.White: return ConsoleColor.White;

				case Color.DefaultForeground: return TextEngine.Story?.DefaultForegroundColor ?? ConsoleColor.Gray;
				case Color.DefaultBackground: return TextEngine.Story?.HighlightBackgroundColor ?? ConsoleColor.Black;
				case Color.HighlightForeground: return TextEngine.Story?.HighlightForegroundColor
						?? TextEngine.Story?.DefaultForegroundColor
						?? ConsoleColor.Gray;
				case Color.HighlightBackground: return TextEngine.Story?.HighlightBackgroundColor
						?? TextEngine.Story?.DefaultBackgroundColor
						?? ConsoleColor.Black;
				case Color.HintForeground: return TextEngine.Story?.HintForegroundColor
						?? TextEngine.Story?.DefaultForegroundColor
						?? ConsoleColor.DarkGray;
				case Color.HintBackground: return TextEngine.Story?.HintBackgroundColor
						?? TextEngine.Story?.DefaultBackgroundColor
						?? ConsoleColor.Black;
				default: throw new ArgumentOutOfRangeException(nameof(color), color, null);
			}
		}
	}
}