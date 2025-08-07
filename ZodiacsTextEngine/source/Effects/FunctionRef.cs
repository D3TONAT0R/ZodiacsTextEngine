using System.Threading.Tasks;
using ZodiacsTextEngine.Parser;

namespace ZodiacsTextEngine.Effects
{
	public class FunctionRef : Effect
	{
		public string functionId;
		public string[] arguments;

		public FunctionRef(string functionId, params string[] arguments)
		{
			this.functionId = functionId;
			this.arguments = arguments;
		}

		public override async Task Execute(EffectGroup g)
		{
			string output = await Functions.Execute(functionId, arguments);
			if(output != null) TextEngine.Interface.Write(output, true);
		}

		public override LogMessage Validate(string site)
		{
			if(!Functions.Exists(functionId)) return LogMessage.Error(site, "Nonexisting function referenced: " + functionId);
			return null;
		}

		[EffectParser("FUNC")]
		public static Effect Parse(EffectParseContext ctx)
		{
			var args = ctx.GetArguments();
			string funcId = args[0];
			args.RemoveAt(0);
			return new FunctionRef(funcId, args.ToArray());
		}
	}
}