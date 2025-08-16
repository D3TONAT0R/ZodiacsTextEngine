using System;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class Todo : Effect
	{
		public string info;

		public Todo(string info)
		{
			this.info = info;
		}

		public override Task Execute(EffectGroup g)
		{
			TextEngine.Interface.Text("TODO: " + info, Color.Magenta);
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			return LogMessage.Warning(site, "TODO marker found: " + info);
		}

		[EffectParser("TODO")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new Todo(ctx.content);
		}
	}
}