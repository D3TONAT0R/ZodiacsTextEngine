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

				case Color.DefaultForeground: return TextEngine.GameData?.DefaultForegroundColor ?? ConsoleColor.Gray;
				case Color.DefaultBackground: return TextEngine.GameData?.HighlightBackgroundColor ?? ConsoleColor.Black;
				case Color.HighlightForeground: return TextEngine.GameData?.HighlightForegroundColor
						?? TextEngine.GameData?.DefaultForegroundColor
						?? ConsoleColor.Gray;
				case Color.HighlightBackground: return TextEngine.GameData?.HighlightBackgroundColor
						?? TextEngine.GameData?.DefaultBackgroundColor
						?? ConsoleColor.Black;
				case Color.HintForeground: return TextEngine.GameData?.HintForegroundColor ?? ConsoleColor.DarkGray;
				case Color.HintBackground: return TextEngine.GameData?.HintBackgroundColor ?? ConsoleColor.Black;
				default: throw new ArgumentOutOfRangeException(nameof(color), color, null);
			}
		}
	}
}