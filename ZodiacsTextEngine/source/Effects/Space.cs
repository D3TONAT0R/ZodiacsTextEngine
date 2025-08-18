using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class Space : Effect
	{
		int count;

		public Space(int count)
		{
			this.count = count;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.VerticalSpace(count);
			return Task.CompletedTask;
		}

		[EffectParser("SPACE")]
		public static Space Parse(EffectParseContext ctx)
		{
			if(ctx.content != null && int.TryParse(ctx.content.Trim(), out int count))
			{
				return new Space(count);
			}
			else
			{
				return new Space(1); // Default to 1 space if no valid count is provided
			}
		}
	}
}