using System;
using System.Collections.Generic;
using System.Text;

namespace ZodiacsTextEngine
{
	public class TextUtility
	{
		private static StringBuilder sb = new StringBuilder();

		public static int GetNextWordLength(string text, int start)
		{
			int i = start;
			while(i < text.Length && text[i] != ' ' && text[i] != '\n') i++;
			return i - start;
		}

		public static string[] GetWordWrappedLines(string text, int maxWidth, ref int xPos)
		{
			List<string> lines = new List<string>();
			sb.Clear();

			int i = 0;
			while(i < text.Length)
			{
				//Reset x position a line break follows
				if(text[i] == '\n')
				{
					lines.Add(sb.ToString());
					sb.Clear();
					xPos = 0;
					i++;
				}
				else
				{
					int nextWord = Math.Max(1, GetNextWordLength(text, i));
					if(i + nextWord <= text.Length)
					{
						if(xPos + nextWord >= maxWidth)
						{
							lines.Add(sb.ToString());
							sb.Clear();
							xPos = 0;

							//Ignore the next space if it follows right after a forced line break
							if(text[i] == ' ' && i < text.Length)
							{
								i++;
								continue;
							}
						}
						sb.Append(text.Substring(i, nextWord));
						i += nextWord;
						xPos += nextWord;
					}
					else
					{
						break;
					}
				}
			}
			lines.Add(sb.ToString());
			return lines.ToArray();
		}
	}
}
