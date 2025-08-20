using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class SetTitle : Effect
	{
		public string title;

		public SetTitle(string title)
		{
			this.title = title;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.SetTitle(title ?? "");
			return Task.CompletedTask;
		}

		[EffectParser("SET_TITLE")]
		public static SetTitle Parse(EffectParseContext ctx)
		{
			return new SetTitle(ctx.content);
		}
	}
}