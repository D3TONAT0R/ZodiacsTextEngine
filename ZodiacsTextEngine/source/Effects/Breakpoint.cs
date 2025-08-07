using System.Diagnostics;
using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class Breakpoint : Effect
	{
		public override Task Execute(EffectGroup g)
		{
			Debugger.Break();
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			return LogMessage.Warning(site, "Breakpoint marker found");
		}

		[EffectParser("BREAKPOINT")]
		public static Effect Parse(EffectParseContext ctx)
		{
			return new Breakpoint();
		}
	}
}