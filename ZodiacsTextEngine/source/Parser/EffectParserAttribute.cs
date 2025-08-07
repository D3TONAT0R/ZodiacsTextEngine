using System;
using System.Collections.Generic;
using System.Text;

namespace ZodiacsTextEngine.Parser
{
	public class EffectParserAttribute : Attribute
	{
		public readonly string identifier;
		public readonly bool multiline;

		public EffectParserAttribute(string identifier, bool multiline = false)
		{
			this.identifier = identifier;
			this.multiline = multiline;
		}	
	}
}
