using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class ModifyStringVariable : Effect
	{
		public string variableName;
		public string value;
		public bool additive;

		public ModifyStringVariable(string variableName, string value, bool additive)
		{
			this.variableName = variableName;
			this.value = value;
			this.additive = additive;
		}

		public override Task Execute(EffectGroup g)
		{
			var store = GameSession.Current.variables;
			if(additive)
			{
				store.AddString(variableName, value);
			}
			else
			{
				store.SetString(variableName, value);
			}
			return Task.CompletedTask;
		}

		public override LogMessage Validate(string site)
		{
			if(!Variables.IsDefined(variableName))
			{
				return LogMessage.Warning(site, $"Undefined variable '{variableName}'");
			}
			return base.Validate(site);
		}

		[EffectParser("SVAR_SET")]
		public static ModifyStringVariable ParseSet(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyStringVariable(args[0], args[1], false);
		}

		[EffectParser("SVAR_ADD")]
		public static ModifyStringVariable ParseAdd(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyStringVariable(args[0], args[1], true);
		}
	}
}