using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine.Parsers
{
	public static class RoomParser
	{
		public class ParserContext
		{
			public Story story;
			public string currentFileName;
			public string[] lines;
			public ConditionalEffectBlock currentBlock = null;

			public ParserContext(Story story, string fileName, string[] lines)
			{
				this.story = story;
				currentFileName = fileName;
				this.lines = lines;
			}
		}

		private enum StatementType
		{
			OnEnter,
			OnExit,
			Choice
		}

		private const string EVENT_MARKER = "@";
		private const string CHOICE_MARKER = ">";
		private const string EFFECT_MARKER = "+";
		private const string CONDITION_MARKER = "IF ";
		private const string END_CONDITION_MARKER = "ENDIF";
		private const string ELSE_CONDITION_MARKER = "ELSE";
		private const string COMMENT_MARKER = "//";

		private static readonly Dictionary<string, EffectParserDelegate> effectParsers = new Dictionary<string, EffectParserDelegate>();

		static RoomParser()
		{
			//Initialize effect parsers
			foreach(var type in typeof(TextEngine).Assembly.GetTypes())
			{
				//Find all methods with EffectParserAttribute
				foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
				{
					try
					{
						var attr = method.GetCustomAttribute<EffectParserAttribute>();
						if(attr != null)
						{
							if(!typeof(Effect).IsAssignableFrom(method.ReturnType) && method.GetParameters().Select(p => p.ParameterType).SequenceEqual(new Type[] { typeof(EffectParseContext) }))
							{
								throw new InvalidOperationException($"Invalid method signature for effect parser '{method.Name}' in {method.DeclaringType.Name}.");
							}
							if(!method.IsStatic)
							{
								throw new InvalidOperationException($"Effect parser method '{method.Name}' in {method.DeclaringType.Name} must be static.");
							}
							var id = attr.identifier.ToUpper();
							if(effectParsers.ContainsKey(id))
							{
								throw new InvalidOperationException($"Duplicate effect parser identifier '{id}' found in {method.DeclaringType.Name} and in {effectParsers[id].Method.DeclaringType.Name}.");
							}
							effectParsers[attr.identifier.ToUpper()] = (EffectParserDelegate)Delegate.CreateDelegate(typeof(EffectParserDelegate), method);
						}
					}
					catch(Exception e)
					{
						TextEngine.Interface.LogError(e.Message);
					}
				}
			}
		}

		public static Room Parse(Story story, string fileName, string content)
		{
			var name = Path.GetFileNameWithoutExtension(fileName);
			var lines = content.Replace("\r", "").Split('\n');
			var ctx = new ParserContext(story, name, lines);

			int linePos = 0;
			Room room = new Room(ctx.currentFileName);
			bool done;
			do
			{
				done = !ParseNextBlock(ctx, ref linePos, ref room);
			}
			while(!done);
			return room;
		}

		private static bool IsEmptyOrComment(ParserContext ctx, int lineIndex)
		{
			return IsEmpty(ctx, lineIndex) || IsComment(ctx, lineIndex);
		}

		private static bool IsEmpty(ParserContext ctx, int lineIndex)
		{
			return ctx.lines[lineIndex].Trim() == "";
		}

		private static bool IsComment(ParserContext ctx, int lineIndex)
		{
			return ctx.lines[lineIndex].TrimStart().StartsWith(COMMENT_MARKER);
		}

		private static bool ParseNextBlock(ParserContext ctx, ref int linePos, ref Room room)
		{
			while(linePos < ctx.lines.Length && IsEmptyOrComment(ctx, linePos))
			{
				MoveNextLine(ctx, ref linePos);
			}
			if(linePos >= ctx.lines.Length)
			{
				//EOF reached
				return false;
			}
			ParseKeyword(ctx, linePos, out var keyword, out var args);
			EffectGroup group;
			switch(keyword)
			{
				case StatementType.OnEnter:
					group = new EffectGroup(room, "onEnter");
					break;
				case StatementType.OnExit:
					group = new EffectGroup(room, "onExit");
					break;
				case StatementType.Choice:
					group = new Choice(room, string.Join(" ", args));
					break;
				default: throw new NotImplementedException();
			}
			MoveNextLine(ctx, ref linePos);
			if(linePos >= ctx.lines.Length) return false;
			while(linePos < ctx.lines.Length && !ctx.lines[linePos].StartsWith(EVENT_MARKER) &&
				!ctx.lines[linePos].StartsWith(CHOICE_MARKER))
			{
				var line = ctx.lines[linePos].TrimStart();
				if(string.IsNullOrWhiteSpace(line))
				{
					MoveNextLine(ctx, ref linePos);
					continue;
				}
				if(line.StartsWith(EFFECT_MARKER))
				{
					/*
					if(group.effects.Count > 0)
					{
						var lastEffect = group.effects.Last();
						if(lastEffect is GoToRoom || lastEffect is GameOver)
						{
							throw new FileParseException(currentFileName, linePos, "GOTO / GAMEOVER must be the last action in a block.");
						}
					}
					*/
					var effect = ParseEffect(ctx, ref linePos);
					if(ctx.currentBlock != null)
					{
						if(ctx.currentBlock.HasInvertedBlock) ctx.currentBlock.AddInvertedChild(effect);
						else ctx.currentBlock.AddChild(effect);
					}
					else
					{
						group.effects.Add(effect);
					}
					MoveNextLine(ctx, ref linePos);
				}
				else if(line.StartsWith(CONDITION_MARKER))
				{
					var c = ParseCondition(ctx, ref linePos);
					var ifBlock = new ConditionalEffectBlock(c);
					ifBlock.parent = ctx.currentBlock;

					if(ctx.currentBlock != null)
					{
						if(ctx.currentBlock.HasInvertedBlock) ctx.currentBlock.AddInvertedChild(ifBlock);
						else ctx.currentBlock.AddChild(ifBlock);
					}
					else
					{
						group.effects.Add(ifBlock);
					}

					ctx.currentBlock = ifBlock;
					MoveNextLine(ctx, ref linePos);
				}
				else if(line.StartsWith(END_CONDITION_MARKER))
				{
					if(ctx.currentBlock == null) throw new FileParseException(ctx, linePos, "ENDIF encountered outside of an IF block.");
					if(ctx.currentBlock.parent != null)
					{
						//Console.WriteLine("Not null: "+linePos);
					}
					ctx.currentBlock = ctx.currentBlock.parent;
					MoveNextLine(ctx, ref linePos);
				}
				else if(line.StartsWith(ELSE_CONDITION_MARKER))
				{
					if(ctx.currentBlock == null) throw new FileParseException(ctx, linePos, "ELSE encountered outside of an IF block.");
					if(ctx.currentBlock.HasInvertedBlock) throw new FileParseException(ctx, linePos, "Multiple ELSE statements encountered.");
					ctx.currentBlock.BeginElseBlock();
					//group.effects.Add(currentBlock);
					MoveNextLine(ctx, ref linePos);
				}
				else throw new FileParseException(ctx, linePos, "Unexpected string: " + line);
			}
			if(ctx.currentBlock != null)
			{
				throw new FileParseException(ctx, linePos, "Unclosed IF block encountered");
			}
			if(keyword == StatementType.OnEnter) room.onEnter = group;
			else if(keyword == StatementType.OnExit) room.onExit = group;
			else if(keyword == StatementType.Choice)
			{
				var choice = (Choice)group;
				if(choice.prompt == "!") room.antiChoice = choice;
				else room.choices.Add((Choice)group);
			}
			return true;
		}

		private static Condition ParseCondition(ParserContext ctx, ref int linePos)
		{
			var line = ctx.lines[linePos].TrimStart();
			var args = line.Substring(CONDITION_MARKER.Length).Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
			if(args.Length < 3) throw new FileParseException(ctx, linePos, "Not enough arguments for IF statement (3 required)");
			return new Condition(args[0], ParseConditionalOperator(ctx, linePos, args[1]), Value.Parse(args[2], true));
		}

		private static Variables.ConditionalOperator ParseConditionalOperator(ParserContext ctx, int lineIndex, string input)
		{
			switch(input)
			{
				case "<": return Variables.ConditionalOperator.LessThan;
				case "<=": return Variables.ConditionalOperator.LessThanOrEqual;
				case "==": return Variables.ConditionalOperator.Equal;
				case ">=": return Variables.ConditionalOperator.GreaterThanOrEqual;
				case ">": return Variables.ConditionalOperator.GreaterThan;
				case "!=": return Variables.ConditionalOperator.NotEqual;

				case "IS": return Variables.ConditionalOperator.StringEquals;
				case "NOT":
				case "ISNOT":
				case "!IS": return Variables.ConditionalOperator.StringNotEquals;
				case "CONTAINS": return Variables.ConditionalOperator.StringContains;
				case "!CONTAINS": return Variables.ConditionalOperator.StringNotContains;
				case "STARTSWITH": return Variables.ConditionalOperator.StringStartsWith;
				case "!STARTSWITH": return Variables.ConditionalOperator.StringNotStartsWith;
				case "ENDSWITH": return Variables.ConditionalOperator.StringEndsWith;
				case "!ENDSWITH": return Variables.ConditionalOperator.StringNotEndsWith;
				default: throw new FileParseException(ctx, lineIndex, "Invalid conditional operator: " + input);
			}
		}

		private static Effect ParseEffect(ParserContext ctx, ref int linePos)
		{
			int startLinePos = linePos;
			var split = ctx.lines[linePos].TrimStart().Substring(1).Split(new char[] { ' ' }, 2);
			string keyword = split[0];
			string content = split.Length > 1 ? split[1] : "";
			//Count number of tabs to determine indent
			int indent = 0;
			while(ctx.lines[linePos][indent] == '\t') indent++;
			if(effectParsers.TryGetValue(keyword, out var parser))
			{
				var attr = parser.Method.GetCustomAttribute<EffectParserAttribute>();
				bool allowMultiline = attr.multiline;
				var parseContext = new EffectParseContext(ctx, startLinePos);
				if(allowMultiline)
				{
					if(!string.IsNullOrWhiteSpace(content)) parseContext.content = content;
					else parseContext.lines = GetTextLines(ctx, ref linePos, indent);
				}
				else
				{
					parseContext.content = content;
				}
				try
				{
					var effect = parser.Invoke(parseContext);
					return effect;
				}
				catch(Exception e)
				{
					throw e.InnerException ?? e;
				}
			}
			else
			{
				//Search custom functions
				if(ctx.story.Functions.ContainsKey(keyword.ToLower()))
				{
					return new FunctionRef(keyword, content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
				}
				else
				{
					throw new FileParseException(ctx, startLinePos, "Invalid effect keyword and not a function: " + keyword);
				}
			}
		}

		private static List<string> GetTextLines(ParserContext ctx, ref int linePos, int indent)
		{
			MoveNextLine(ctx, ref linePos);
			List<string> textLines = new List<string>();
			//Get all text lines below the keyword
			do
			{
				if(linePos >= ctx.lines.Length) break;
				string textLine = ctx.lines[linePos];
				for(int i = 0; i < indent; i++)
				{
					if(textLine.StartsWith("\t")) textLine = textLine.Substring(1);
					else break;
				}
				if(textLine.StartsWith(EVENT_MARKER)
					|| textLine.StartsWith(CHOICE_MARKER)
					|| textLine.StartsWith(EFFECT_MARKER)
					|| textLine.StartsWith(CONDITION_MARKER)
					|| textLine.StartsWith(ELSE_CONDITION_MARKER)
					|| textLine.StartsWith(END_CONDITION_MARKER))
				{
					break;
				}
				textLines.Add(textLine);
				MoveNextLine(ctx, ref linePos);
			}
			while(linePos < ctx.lines.Length);
			/*
			while(linePos < lines.Length
				&& !lines[linePos].TrimStart().StartsWith(EVENT_MARKER)
				&& !lines[linePos].TrimStart().StartsWith(CHOICE_MARKER)
				&& !lines[linePos].TrimStart().StartsWith(EFFECT_MARKER)
				&& !lines[linePos].TrimStart().StartsWith(CONDITION_MARKER)
				&& !lines[linePos].TrimStart().StartsWith(ELSE_CONDITION_MARKER)
				&& !lines[linePos].TrimStart().StartsWith(END_CONDITION_MARKER))
			{
				string textLine = lines[linePos];
				for(int i = 0; i < indent; i++)
				{
					if(textLine.StartsWith('\t')) textLine = textLine.Substring(1);
					else break;
				}
				textLines.Add(textLine);
				MoveNextLine(ref linePos);
			}
			*/
			linePos--;
			//Remove whitespace lines at the end
			while(textLines.Count > 0 && string.IsNullOrWhiteSpace(textLines[textLines.Count - 1])) textLines.RemoveAt(textLines.Count - 1);
			return textLines;
		}

		private static void ParseKeyword(ParserContext ctx, int lineNumber, out StatementType keyword, out string[] args)
		{
			keyword = GetStatementType(ctx, lineNumber, out int keyLength);

			var line = ctx.lines[lineNumber];
			var split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> argsList = new List<string>(split);
			if(keyword == StatementType.Choice) argsList[0] = argsList[0].Substring(1);
			else argsList.RemoveAt(0);
			args = argsList.ToArray();
		}

		private static bool MoveNextLine(ParserContext ctx, ref int lineIndex)
		{
			lineIndex++;
			if(lineIndex >= ctx.lines.Length) return false;
			while(IsComment(ctx, lineIndex))
			{
				lineIndex++;
				if(lineIndex >= ctx.lines.Length) return false;
			}
			return true;
		}

		private static StatementType GetStatementType(ParserContext ctx, int lineNumber, out int offset)
		{
			var input = ctx.lines[lineNumber];
			if(input.StartsWith(EVENT_MARKER))
			{
				input = input.Substring(1);
				if(input.StartsWith("ENTER"))
				{
					offset = 7;
					return StatementType.OnEnter;
				}
				else if(input.StartsWith("EXIT"))
				{
					offset = 6;
					return StatementType.OnExit;
				}
				else throw new FileParseException(ctx, lineNumber, "Invalid statement type: " + input.Split(' ')[0]);
			}
			else if(input.StartsWith(CHOICE_MARKER))
			{
				offset = 1;
				return StatementType.Choice;
			}
			else throw new FileParseException(ctx, lineNumber, $"Invalid statement '{input}'");
		}

		public static Color ParseColor(ParserContext ctx, string input, int linePos)
		{
			switch(input.ToLower())
			{
				case "black": return Color.Black;
				case "blue": return Color.Blue;
				case "cyan": return Color.Cyan;
				case "dark_blue": return Color.DarkBlue;
				case "dark_cyan": return Color.DarkCyan;
				case "dark_gray": return Color.DarkGray;
				case "dark_green": return Color.DarkGreen;
				case "dark_magenta": return Color.DarkMagenta;
				case "dark_red": return Color.DarkRed;
				case "dark_yellow": return Color.DarkYellow;
				case "gray": return Color.Gray;
				case "green": return Color.Green;
				case "magenta": return Color.Magenta;
				case "red": return Color.Red;
				case "white": return Color.White;
				case "yellow": return Color.Yellow;
				case "text": return Color.DefaultForeground;
				case "background": return Color.DefaultBackground;
				case "highlight_text": return Color.HighlightForeground;
				case "highlight_background": return Color.HighlightBackground;
				case "hint_text": return Color.HighlightForeground;
				case "hint_background": return Color.HintBackground;
				default: throw new FileParseException(ctx, linePos, "Invalid input for color: " + input);
			}
		}

		public static ConsoleColor ParseConsoleColor(ParserContext ctx, string input, int linePos)
		{
			switch(input.ToLower())
			{
				case "black": return ConsoleColor.Black;
				case "blue": return ConsoleColor.Blue;
				case "cyan": return ConsoleColor.Cyan;
				case "dark_blue": return ConsoleColor.DarkBlue;
				case "dark_cyan": return ConsoleColor.DarkCyan;
				case "dark_gray": return ConsoleColor.DarkGray;
				case "dark_green": return ConsoleColor.DarkGreen;
				case "dark_magenta": return ConsoleColor.DarkMagenta;
				case "dark_red": return ConsoleColor.DarkRed;
				case "dark_yellow": return ConsoleColor.DarkYellow;
				case "gray": return ConsoleColor.Gray;
				case "green": return ConsoleColor.Green;
				case "magenta": return ConsoleColor.Magenta;
				case "red": return ConsoleColor.Red;
				case "white": return ConsoleColor.White;
				case "yellow": return ConsoleColor.Yellow;
				default: throw new FileParseException(ctx, linePos, "Invalid input for console color: " + input);
			}
		}
	}

	public class FileParseException : Exception
	{
		private readonly string fileName;
		private readonly int lineNumber;
		private readonly string message;
		private readonly string line;

		public override string Message => $"File parse error on file '{fileName}' at line {lineNumber}: {message}\n'{line}'";

		public FileParseException(RoomParser.ParserContext ctx, int lineIndex, string message)
		{
			fileName = ctx.currentFileName;
			lineNumber = lineIndex + 1;
			this.message = message;
			line = lineIndex >= 0 && lineIndex < ctx.lines.Length ? ctx.lines[lineIndex].TrimEnd() : "";
			if(line.Length > 50) line = line.Substring(0, 47) + "...";
		}
	}
}
