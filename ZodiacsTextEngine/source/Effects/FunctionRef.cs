using System.Threading.Tasks;
using ZodiacsTextEngine.Parsers;

namespace ZodiacsTextEngine.Effects
{
	public class FunctionRef : Effect
	{
		public string functionId;
		public FunctionArgs args;

		public FunctionRef(string functionId, FunctionArgs args)
		{
			this.functionId = functionId;
			this.args = args;
		}

		public override async Task Execute(EffectGroup g)
		{
			string output = await Functions.Execute(functionId, args);
			if(output != null) TextEngine.Interface.Write(output, true);
		}

		public override LogMessage Validate(string site)
		{
			if(!Functions.Exists(functionId)) return LogMessage.Error(site, "Nonexistent function referenced: " + functionId);
			return null;
		}

		[EffectParser("FUNC")]
		public static Effect Parse(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			string funcId = args[0];
			args.RemoveAt(0);
			return new FunctionRef(funcId, new FunctionArgs(ctx.content));
		}
	}
}