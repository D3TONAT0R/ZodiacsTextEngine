using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	//TODO add support for (var + var)
	public class ModifyStringVariable : Effect
	{
		public string variableName;
		public Value value;
		public bool additive;

		public ModifyStringVariable(string variableName, Value value, bool additive)
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
				store.AddString(variableName, value.GetString());
			}
			else
			{
				store.SetString(variableName, value.GetString());
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
			return new ModifyStringVariable(args[0], Value.Parse(ctx.content.Substring(args[0].Length).Trim(), true), false);
		}

		[EffectParser("SVAR_ADD")]
		public static ModifyStringVariable ParseAdd(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			return new ModifyStringVariable(args[0], Value.Parse(ctx.content.Substring(args[0].Length).Trim(), true), true);
		}

		private static string GetStringInQuotes(EffectParseContext ctx, string text)
		{
			//Return string between quotes via regex
			var match = System.Text.RegularExpressions.Regex.Match(text, "\"([^\"]*)\"");
			if(!match.Success)
			{
				throw new FileParseException(ctx.parserContext, ctx.startLinePos, "No valid string found");
			}
			return "\"" + match.Groups[1].Value + "\"";
		}
	}
}