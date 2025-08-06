using System.Collections.Generic;

namespace ZodiacsTextEngine
{
	public class Variables
	{
		public enum ConditionalOperator
		{
			LessThan,
			LessThanOrEqual,
			Equal,
			GreaterThanOrEqual,
			GreaterThan,
			NotEqual
		}

		public static bool HasFixedVariableNames => fixedVariableNames != null;

		public static List<string> fixedVariableNames = null;
		private Dictionary<string, int> data;

		public static void Init(List<string> fixedVariableNames)
		{
			Variables.fixedVariableNames = fixedVariableNames;
		}

		public Variables()
		{
			data = new Dictionary<string, int>();
		}

		public void Clear()
		{
			data.Clear();
		}

		public void Set(string name, int value)
		{
			data[name] = value;
		}

		public int Get(string name)
		{
			if(data.ContainsKey(name))
			{
				return data[name];
			}
			else
			{
				return 0;
			}
		}

		public void Add(string name, int value)
		{
			if(data.ContainsKey(name))
			{
				data[name] += value;
			}
			else
			{
				data[name] = value;
			}
		}

		public bool Check(string name, ConditionalOperator operation, int value)
		{
			var variableValue = Get(name);
			switch(operation)
			{
				case ConditionalOperator.LessThan:
					return variableValue < value;
				case ConditionalOperator.LessThanOrEqual:
					return variableValue <= value;
				case ConditionalOperator.Equal:
					return variableValue == value;
				case ConditionalOperator.GreaterThanOrEqual:
					return variableValue >= value;
				case ConditionalOperator.GreaterThan:
					return variableValue > value;
				case ConditionalOperator.NotEqual:
					return variableValue != value;
				default:
					return true;
			}
		}

		public static bool IsDefined(string name)
		{
			if(fixedVariableNames != null)
			{
				return fixedVariableNames.Contains(name);
			}
			else return true;
		}
	}
}
