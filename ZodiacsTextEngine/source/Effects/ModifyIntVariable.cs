using System.Threading.Tasks;

namespace ZodiacsTextEngine.Effects
{
	public class ModifyIntVariable : Effect
	{
		public string variableName;
		public int value;
		public bool additive;

		public ModifyIntVariable(string variableName, int value, bool additive)
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
				store.AddInt(variableName, value);
			}
			else
			{
				store.SetInt(variableName, value);
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
	}
}