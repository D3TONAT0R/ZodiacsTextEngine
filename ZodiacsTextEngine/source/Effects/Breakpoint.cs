using System.Diagnostics;
using System.Threading.Tasks;

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
	}
}