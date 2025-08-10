using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	//TODO add support for (var + var) and (var - var)
	public class ModifyIntVariable : Effect
	{
		public string variableName;
		public IValue value;
		public bool additive;

		public ModifyIntVariable(string variableName, IValue value, bool additive)
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
				store.AddInt(variableName, value.GetValue());
			}
			else
			{
				store.SetInt(variableName, value.GetValue());
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
			return new ModifyIntVariable(args[0], ParseValue(args[1]), false);
		}

		[EffectParser("VAR_ADD")]
		public static ModifyIntVariable ParseAdd(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyIntVariable(args[0], ParseValue(args[1]), true);
		}

		private static IValue ParseValue(string value)
		{
			if(value.Length > 1 && value[0] == '-' && (char.IsLetter(value[1]) || value[1] == '_'))
			{
				//Negative variable reference
				return new VariableRef(value.Substring(1), false);
			}
			if(char.IsLetter(value[0]) || value[0] == '_')
			{
				//Parse variable reference
				return new VariableRef(value, true);
			}
			return new Constant(int.Parse(value));
		}
	}
}