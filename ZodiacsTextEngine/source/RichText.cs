using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine
{
	public abstract class RichTextComponent
	{
		public string text;

		protected RichTextComponent(string text)
		{
			this.text = text;
		}

		protected virtual void BeginWrite(Color baseForegroundColor, Color baseBackgroundColor)
		{

		}

		protected virtual void EndWrite()
		{

		}

		public virtual Task Write(Color baseForegroundColor, Color baseBackgroundColor)
		{
			BeginWrite(baseForegroundColor, baseBackgroundColor);
			if(text != null) TextEngine.Interface.Write(text, false);
			EndWrite();
			return Task.CompletedTask;
		}
	}

	public class PlainTextComponent : RichTextComponent
	{
		public PlainTextComponent(string text) : base(text)
		{

		}
	}

	public class SetColorComponent : RichTextComponent
	{
		public Color foregroundColor;

		public SetColorComponent(Color foregroundColor) : base(null)
		{
			this.foregroundColor = foregroundColor;
		}

		protected override void BeginWrite(Color baseForegroundColor, Color baseBackgroundColor)
		{
			TextEngine.Interface.ForegroundColor = foregroundColor;
		}
	}

	public class SetBackgroundColorComponent : RichTextComponent
	{
		public Color backgroundColor;
		public SetBackgroundColorComponent(Color backgroundColor) : base(null)
		{
			this.backgroundColor = backgroundColor;
		}
		protected override void BeginWrite(Color baseForegroundColor, Color baseBackgroundColor)
		{
			TextEngine.Interface.BackgroundColor = backgroundColor;
		}
	}

	public class ResetColorComponent : RichTextComponent
	{
		public ResetColorComponent() : base(null)
		{

		}

		protected override void BeginWrite(Color baseForegroundColor, Color baseBackgroundColor)
		{
			TextEngine.Interface.ForegroundColor = baseForegroundColor;
			TextEngine.Interface.BackgroundColor = baseBackgroundColor;
		}
	}

	public class VariableComponent : RichTextComponent
	{
		public string variableName;
		public float multiplier = 1;
		public string format;

		public VariableComponent(string variableName, float multiplier = 1, string format = null) : base(null)
		{
			this.variableName = variableName;
			this.multiplier = multiplier;
			this.format = format;
		}

		public override Task Write(Color baseForegroundColor, Color baseBackgroundColor)
		{
			//Get the value of the variable and multiply it by the multiplier, with up to 2 decimal places
			float variable = GameSession.Current.variables.GetInt(variableName);
			//Get value as a string with the given format
			string value = (variable * multiplier).ToString(format ?? "0.##");
			TextEngine.Interface.Write(value, false);
			return Task.CompletedTask;
		}
	}

	public class StringVariableComponent : RichTextComponent
	{
		public enum Case
		{
			Unchanged,
			Upper,
			Lower
		}

		public string variableName;
		public string defaultValue;
		public Case textCase;

		public StringVariableComponent(string variableName, string defaultValue = null, Case textCase = Case.Unchanged) : base(null)
		{
			this.variableName = variableName;
			this.textCase = textCase;
		}

		public override Task Write(Color baseForegroundColor, Color baseBackgroundColor)
		{
			var text = (GameSession.Current.variables.GetString(variableName) ?? defaultValue) ?? "";
			switch(textCase)
			{
				case Case.Upper:
					text = text.ToUpperInvariant();
					break;
				case Case.Lower:
					text = text.ToLowerInvariant();
					break;
			}
			TextEngine.Interface.Write(text, false);
			return Task.CompletedTask;
		}
	}

	public class FunctionComponent : RichTextComponent
	{
		public string functionName;
		public string[] arguments;

		public FunctionComponent(string functionName, string[] arguments) : base(null)
		{
			this.functionName = functionName;
			this.arguments = arguments;
		}

		public override async Task Write(Color baseForegroundColor, Color baseBackgroundColor)
		{
			string output = await Functions.Execute(functionName, arguments);
			if(output != null) TextEngine.Interface.Write(output, false);
		}
	}

	public class HighlightComponent : RichTextComponent
	{
		public HighlightComponent() : base(null)
		{

		}

		protected override void BeginWrite(Color baseForegroundColor, Color baseBackgroundColor)
		{
			var gameData = TextEngine.GameData;
			if(gameData.HighlightForegroundColor.HasValue)
			{
				TextEngine.Interface.ForegroundColor = Color.HighlightForeground;
			}
			if(gameData.HighlightBackgroundColor.HasValue)
			{
				TextEngine.Interface.BackgroundColor = Color.HighlightBackground;
			}
		}
	}

	public class RichText
	{
		public List<RichTextComponent> components;

		public RichText(params RichTextComponent[] components)
		{
			this.components = new List<RichTextComponent>(components);
		}

		public void AddComponent(RichTextComponent component)
		{
			components.Add(component);
		}

		public void Write(bool lineBreak = true)
		{
			var baseForegroundColor = TextEngine.Interface.ForegroundColor;
			var baseBackgroundColor = TextEngine.Interface.BackgroundColor;
			foreach(var component in components)
			{
				component.Write(baseForegroundColor, baseBackgroundColor);
			}
			TextEngine.Interface.ForegroundColor = baseForegroundColor;
			TextEngine.Interface.BackgroundColor = baseBackgroundColor;
			if(lineBreak) TextEngine.Interface.LineBreak();
		}

		public static RichText Parse(RoomParser.ParserContext ctx, string text, int startLine)
		{
			var richText = new RichText();
			//Find all matches of pattern <*>, with * being any character except space
			MatchCollection matches = Regex.Matches(text, @"<[^ ]+?>");
			int startIndex = 0;
			for(int i = 0; i <= matches.Count; i++)
			{
				if(i < matches.Count)
				{
					var match = matches[i];
					//Add the text before the match
					if(match.Index > 0)
					{
						richText.AddComponent(new PlainTextComponent(text.Substring(startIndex, match.Index - startIndex)));
					}
					startIndex = match.Index + match.Length;
					//Remove the brackets from the match
					string matchText = match.Value.Substring(1, match.Value.Length - 2).ToLower();
					//Get the type of the match (<color=*> or <bgcolor=*> or <clear>)
					if(matchText.StartsWith("color=") || matchText.StartsWith("c="))
					{
						richText.AddComponent(new SetColorComponent(RoomParser.ParseColor(ctx, matchText.Split('=')[1], startLine)));
					}
					else if(matchText.StartsWith("bgcolor=") || matchText.StartsWith("b="))
					{
						richText.AddComponent(new SetBackgroundColorComponent(RoomParser.ParseColor(ctx, matchText.Split('=')[1], startLine)));
					}
					else if(matchText == "highlight" || matchText == "h")
					{
						richText.AddComponent(new HighlightComponent());
					}
					else if(matchText == "reset" || matchText == "r")
					{
						richText.AddComponent(new ResetColorComponent());
					}
					else if(matchText.StartsWith("var="))
					{
						// <var=fuel> or <var=fuel*2.5> or <var=fuel*2.5@3> or <var=fuel@3>
						// *2.5 = multiplier
						// @3 = decimal places
						var varData = matchText.Split('=')[1];
						var varName = varData.Split(new char[] { '*', '@' })[0];

						float multiplier = 1;
						//Get the multiplier using a regex expression
						Match multiplierMatch = Regex.Match(varData, @"\*(\d+(\.\d+)?)");
						if(multiplierMatch.Success)
						{
							multiplier = float.Parse(multiplierMatch.Groups[1].Value);
						}

						string format = null;
						//Get the decimal place count defined by @<number> if available
						Match decimalsFormatMatch = Regex.Match(varData, @"@(\d+)");
						if(decimalsFormatMatch.Success)
						{
							int decimals = int.Parse(decimalsFormatMatch.Groups[1].Value);
							format = "F" + decimals; // Format for fixed-point notation with specified decimal places
						}
						else
						{
							//Get anything after a colon as the format
							Match formatMatch = Regex.Match(varData, @":(.*)");
							if(formatMatch.Success)
							{
								format = formatMatch.Groups[1].Value;
							}
						}

						richText.AddComponent(new VariableComponent(varName, multiplier, format));
					}
					else if(matchText.StartsWith("svar="))
					{
						// <svar=name> or <svar=name@U> or <var=name@L>
						// @U or @L = force upper or lower case
						var varData = matchText.Split('=')[1];
						var varName = varData.Split(new char[] { '@' })[0];

						//Get the decimal place count defined by @<number> if available
						Match caseMatch = Regex.Match(varData, @"@(\d+)");
						StringVariableComponent.Case textCase = StringVariableComponent.Case.Unchanged;
						if(caseMatch.Success)
						{
							var v = caseMatch.Groups[1].Value.ToUpper();
							if(v == "U") textCase = StringVariableComponent.Case.Upper;
							else if(v == "L") textCase = StringVariableComponent.Case.Lower;
							else throw new FileParseException(ctx, startLine, $"Invalid case specifier: '{v}'");
						}
						richText.AddComponent(new StringVariableComponent(varName, null, textCase));
					}
					else if(matchText.StartsWith("func"))
					{
						var funcData = matchText.Split('=')[1];
						//Get the function name before the first parentheses
						var funcName = funcData.Split('(')[0];
						//Get everything between parentheses as arguments
						var argsMatch = Regex.Match(funcData, @"\((.*)\)");
						string[] funcArgs;
						if(argsMatch.Success)
						{
							funcArgs = argsMatch.Groups[1].Value.Split(',');
						}
						else
						{
							funcArgs = Array.Empty<string>();
						}
						richText.AddComponent(new FunctionComponent(funcName, funcArgs));
					}
					else throw new FileParseException(ctx, startLine, "Invalid rich text tag: " + matchText);
				}
				else
				{
					//Add the remaining text
					if(startIndex < text.Length)
					{
						richText.AddComponent(new PlainTextComponent(text.Substring(startIndex)));
					}
				}
			}
			return richText;
		}
	}
}
