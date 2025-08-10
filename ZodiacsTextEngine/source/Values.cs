namespace ZodiacsTextEngine
{
	public interface IValue
	{
		int GetValue();
	}

	public class Constant : IValue
	{
		public readonly int value;

		public Constant(int value)
		{
			this.value = value;
		}

		public int GetValue() => value;
	}

	public class VariableRef : IValue
	{
		public readonly string variableName;
		public readonly bool sign;

		public VariableRef(string variableName, bool sign)
		{
			this.variableName = variableName;
			this.sign = sign;
		}

		public int GetValue()
		{
			int v = GameSession.Current.variables.GetInt(variableName);
			if(!sign) v = -v;
			return v;
		}
	}
}