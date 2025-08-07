using System.Collections.Generic;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class WriteText : Effect, ITextEffect
	{
		public string[] lines;

		public WriteText(params string[] lines)
		{
			this.lines = lines;
		}

		public override Task Execute(EffectGroup g)
		{
			foreach(var line in lines)
			{
				TextEngine.Interface.Write(line, true);
			}
			return Task.CompletedTask;
		}

		public IEnumerable<string> GetTextStrings()
		{
			yield return string.Join("\n", lines);
		}

		[EffectParser("PLAIN_TEXT", true)]
		public static Effect Parse(EffectParseContext ctx)
		{
			if(!string.IsNullOrWhiteSpace(ctx.content)) return new WriteText(ctx.content);
			else
			{
				if(ctx.lines == null || ctx.lines.Count == 0) throw new FileParseException(ctx.parserContext, ctx.startLinePos, "Text component does not contain any text");
				return new WriteText(ctx.lines.ToArray());
			}
		}
	}
}