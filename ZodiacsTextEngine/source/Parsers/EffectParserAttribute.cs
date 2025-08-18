using System;
using ZodiacsTextEngine.Effects;

namespace ZodiacsTextEngine.Parsers
{
	public delegate Effect EffectParserDelegate(EffectParseContext context);

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
