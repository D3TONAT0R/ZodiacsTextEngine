using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class ChangeColorSetting : Effect
	{
		public enum SettingType
		{
			Foreground,
			Background,
			HighlightForeground,
			HighlightBackground,
			HintForeground,
			HintBackground,
			InputForeground,
			InputBackground
		}

		public SettingType setting;
		public ConsoleColor? color;

		public ChangeColorSetting(SettingType setting, ConsoleColor? color = null)
		{
			this.setting = setting;
			this.color = color;
		}

		public override Task Execute(EffectGroup g)
		{
			switch(setting)
			{
				case SettingType.Foreground:
					TextEngine.GameData.DefaultForegroundColor = color ?? ConsoleColor.Gray;
					break;
				case SettingType.Background:
					TextEngine.GameData.DefaultBackgroundColor = color ?? ConsoleColor.Black;
					break;
				case SettingType.HighlightForeground:
					TextEngine.GameData.HighlightForegroundColor = color;
					break;
				case SettingType.HighlightBackground:
					TextEngine.GameData.HighlightBackgroundColor = color;
					break;
				case SettingType.HintForeground:
					TextEngine.GameData.HintForegroundColor = color;
					break;
				case SettingType.HintBackground:
					TextEngine.GameData.HintBackgroundColor = color;
					break;
				case SettingType.InputForeground:
					TextEngine.GameData.InputForegroundColor = color;
					break;
				case SettingType.InputBackground:
					TextEngine.GameData.InputBackgroundColor = color;
					break;
			}
			return Task.CompletedTask;
		}

		[EffectParser("COLOR_SETTING")]
		public static ChangeColorSetting Parse(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			string settingString = args[0].ToLower();
			SettingType setting;
			switch(settingString)
			{
				case "foreground":
					setting = SettingType.Foreground;
					break;
				case "background":
					setting = SettingType.Background;
					break;
				case "highlight_foreground":
					setting = SettingType.HighlightForeground;
					break;
				case "highlight_background":
					setting = SettingType.HighlightBackground;
					break;
				case "hint_foreground":
					setting = SettingType.HintForeground;
					break;
				case "hint_background":
					setting = SettingType.HintBackground;
					break;
				case "input_foreground":
					setting = SettingType.InputForeground;
					break;
				case "input_background":
					setting = SettingType.InputBackground;
					break;
				default:
					throw new FileParseException(ctx.parserContext, ctx.startLinePos, $"Unknown setting '{settingString}'");
			}
			string colorString = args[1].ToLower();
			ConsoleColor? color;
			if(colorString == "null" || colorString == "none")
			{
				color = null;
			}
			else
			{
				color = RoomParser.ParseConsoleColor(ctx.parserContext, colorString, ctx.startLinePos);
			}
			return new ChangeColorSetting(setting, color);
		}
	}
}