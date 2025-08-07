using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZodiacsTextEngine
{
	public static class RoomParser
	{
		public class ParserContext
		{
			public string currentFileName;
			public string[] lines;
			public ConditionalEffectBlock currentBlock = null;

			public ParserContext(string fileName, string[] lines)
			{
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

		public static Room Parse(string fileName, string content)
		{
			var name = Path.GetFileNameWithoutExtension(fileName);
			var lines = content.Replace("\r", "").Split('\n');
			var ctx = new ParserContext(name, lines);

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
					group = new RoomEvent(room, "onEnter");
					break;
				case StatementType.OnExit:
					group = new RoomEvent(room, "onExit");
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
			if(keyword == StatementType.OnEnter) room.onEnter = (RoomEvent)group;
			else if(keyword == StatementType.OnExit) room.onExit = (RoomEvent)group;
			else if(keyword == StatementType.Choice)
			{
				var choice = (Choice)group;
				if(choice.prompt == "!") room.incorrectChoice = choice;
				else room.choices.Add((Choice)group);

			}
			return true;
		}

		private static Condition ParseCondition(ParserContext ctx, ref int linePos)
		{
			var line = ctx.lines[linePos].TrimStart();
			var args = line.Substring(CONDITION_MARKER.Length).Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
			if(args.Length < 3) throw new FileParseException(ctx, linePos, "Not enough arguments for IF statement (3 required)");
			return new Condition(args[0], ParseConditionalOperator(ctx, linePos, args[1]), args[2]);
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
				case "**": return Variables.ConditionalOperator.StringContains;
				case "!**": return Variables.ConditionalOperator.StringNotContains;
				case ".*": return Variables.ConditionalOperator.StringStartsWith;
				case "!.*": return Variables.ConditionalOperator.StringNotStartsWith;
				case "*.": return Variables.ConditionalOperator.StringEndsWith;
				case "!*.": return Variables.ConditionalOperator.StringNotEndsWith;
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
			if(keyword == "PLAIN_TEXT")
			{
				if(!string.IsNullOrWhiteSpace(content)) return new TextWriter(content);
				else
				{
					List<string> textLines = GetTextLines(ctx, ref linePos, indent);
					if(textLines.Count == 0) throw new FileParseException(ctx, startLinePos, "Text component does not contain any text");
					return new TextWriter(textLines.ToArray());
				}
			}
			else if(keyword == "TEXT")
			{
				if(!string.IsNullOrWhiteSpace(content)) return new RichTextWriter(ParseRichText(ctx, content, linePos));
				else
				{
					int firstTextLinePos = linePos + 1;
					List<string> textLines = GetTextLines(ctx, ref linePos, indent);
					if(textLines.Count == 0) throw new FileParseException(ctx, startLinePos, "Text component does not contain any text");
					return new RichTextWriter(ParseRichText(ctx, string.Join("\n", textLines), firstTextLinePos));
				}
			}
			else if(keyword == "GOTO")
			{
				return new GoToRoom(content.Split(' ')[0]);
			}
			else if(keyword == "WAIT")
			{
				return new WaitForAnyKey();
			}
			else if(keyword == "DELAY")
			{
				return new WaitForSeconds(float.Parse(content.Trim()));
			}
			else if(keyword == "SPACE")
			{
				string input = content.Trim();
				if(!string.IsNullOrEmpty(input))
				{
					return new Space(int.Parse(input));
				}
				else
				{
					return new Space(1);
				}
			}
			else if(keyword == "FUNC")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				string funcId = args[0];
				args.RemoveAt(0);
				return new FunctionRef(funcId, args.ToArray());
			}
			else if(keyword == "VAR_SET")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new ModifyIntVariable(args[0], int.Parse(args[1]), false);
			}
			else if(keyword == "VAR_ADD")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new ModifyIntVariable(args[0], int.Parse(args[1]), true);
			}
			else if(keyword == "SVAR_SET")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new ModifyStringVariable(args[0], args[1], false);
			}
			else if(keyword == "SVAR_ADD")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new ModifyStringVariable(args[0], args[1], true);
			}
			else if(keyword == "COLOR")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new SetColor(ParseConsoleColor(ctx, args[0], linePos));
			}
			else if(keyword == "BACKGROUND")
			{
				var args = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return new SetBackgroundColor(ParseConsoleColor(ctx, args[0], linePos));
			}
			else if(keyword == "RESET_COLOR")
			{
				return new ResetColor();
			}
			else if(keyword == "CLEAR_OUTPUT")
			{
				return new ClearOutput();
			}
			else if(keyword == "TODO")
			{
				return new Todo(content);
			}
			else if(keyword == "BREAKPOINT")
			{
				return new Breakpoint();
			}
			else if(keyword == "GAME_OVER")
			{
				return new GameOver(content);
			}
			else throw new FileParseException(ctx, startLinePos, "Invalid keyword: " + keyword);
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

		private static RichText ParseRichText(ParserContext ctx, string text, int startLine)
		{
			ConsoleColor? foregroundColor = null;
			ConsoleColor? backgroundColor = null;
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
						richText.AddComponent(new RichTextComponent(text.Substring(startIndex, match.Index - startIndex), foregroundColor, backgroundColor));
					}
					startIndex = match.Index + match.Length;
					//Remove the brackets from the match
					string matchText = match.Value.Substring(1, match.Value.Length - 2).ToLower();
					//Get the type of the match (<color=*> or <bgcolor=*> or <clear>)
					if(matchText.StartsWith("color="))
					{
						foregroundColor = ParseConsoleColor(ctx, matchText.Split('=')[1], startLine);
					}
					else if(matchText.StartsWith("bgcolor="))
					{
						backgroundColor = ParseConsoleColor(ctx, matchText.Split('=')[1], startLine);
					}
					else if(matchText == "clear")
					{
						foregroundColor = null;
						backgroundColor = null;
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
						richText.AddComponent(new RichTextComponent(text.Substring(startIndex), foregroundColor, backgroundColor));
					}
				}
			}
			return richText;
		}

		private static ConsoleColor ParseConsoleColor(ParserContext ctx, string input, int linePos)
		{
			switch(input.ToUpper())
			{
				case "BLACK": return ConsoleColor.Black;
				case "BLUE": return ConsoleColor.Blue;
				case "CYAN": return ConsoleColor.Cyan;
				case "DARK_BLUE": return ConsoleColor.DarkBlue;
				case "DARK_CYAN": return ConsoleColor.DarkCyan;
				case "DARK_GRAY": return ConsoleColor.DarkGray;
				case "DARK_GREEN": return ConsoleColor.DarkGreen;
				case "DARK_MAGENTA": return ConsoleColor.DarkMagenta;
				case "DARK_RED": return ConsoleColor.DarkRed;
				case "DARK_YELLOW": return ConsoleColor.DarkYellow;
				case "GRAY": return ConsoleColor.Gray;
				case "GREEN": return ConsoleColor.Green;
				case "MAGENTA": return ConsoleColor.Magenta;
				case "RED": return ConsoleColor.Red;
				case "WHITE": return ConsoleColor.White;
				case "YELLOW": return ConsoleColor.Yellow;
				default: throw new FileParseException(ctx, linePos, "Invalid input for color: " + input);
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
