using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class WriteRichText : Effect, ITextEffect
	{
		public RichText text;

		public WriteRichText(RichText text)
		{
			this.text = text;
		}

		public override Task Execute(EffectGroup g)
		{
			text.Write();
			return Task.CompletedTask;
		}

		public IEnumerable<string> GetTextStrings()
		{
			yield return string.Join(" ", text.components.Select(c => c.text));
		}

		[EffectParser("TEXT", true)]
		public static Effect Parse(EffectParseContext ctx)
		{
			if(!string.IsNullOrWhiteSpace(ctx.content)) return new WriteRichText(RichText.Parse(ctx.parserContext, ctx.content, ctx.startLinePos));
			else
			{
				if(ctx.lines == null || ctx.lines.Count == 0) throw new FileParseException(ctx.parserContext, ctx.startLinePos, "Text component does not contain any text");
				return new WriteRichText(RichText.Parse(ctx.parserContext, string.Join("\n", ctx.lines), ctx.startLinePos + 1));
			}
		}
	}
}