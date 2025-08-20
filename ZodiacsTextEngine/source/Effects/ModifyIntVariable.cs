using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class ModifyIntVariable : Effect
	{
		public string variableName;
		public Value value;
		public bool additive;

		public ModifyIntVariable(string variableName, Value value, bool additive)
		{
			this.variableName = variableName;
			this.value = value;
			this.additive = additive;
		}

		public override Task Execute(EffectGroup g)
		{
			var store = Session.Current.variables;
			if(additive)
			{
				store.AddInt(variableName, value.GetInt());
			}
			else
			{
				store.SetInt(variableName, value.GetInt());
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

		[EffectParser("VAR_SET")]
		public static ModifyIntVariable ParseSet(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyIntVariable(args[0], Value.Parse(args[1], false), false);
		}

		[EffectParser("VAR_ADD")]
		public static ModifyIntVariable ParseAdd(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyIntVariable(args[0], Value.Parse(args[1], false), true);
		}
	}
}