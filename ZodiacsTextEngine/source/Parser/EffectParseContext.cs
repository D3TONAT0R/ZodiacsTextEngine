using System;
using System.Collections.Generic;
using System.Linq;

namespace ZodiacsTextEngine.Parser
{
	public class EffectParseContext
	{
		public int startLinePos;

		public RoomParser.ParserContext parserContext;
		public string content;
		public List<string> lines;

		public EffectParseContext(RoomParser.ParserContext parserContext, int startLinePos)
		{
			this.parserContext = parserContext;
			this.startLinePos = startLinePos;
		}

		public List<string> GetArguments(int count = 10)
		{
			return content.Split(new char[] { ' ' }, count, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}